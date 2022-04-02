﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using Website.Services.SettingsProviders;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Website.Repository;
using Website.Services;

namespace Website.Services
{
	/// <summary>
	/// Сервис должен считать количество запросов за последнее время с каждого IP адреса.
	/// Если число запросов превышает лимит - контроллер будет возвращать http 429 - Too many requests.
	/// Идея состоит в хранение словаря IP-Int, где IP - адрес, с которого выполнялись запросы,
	/// int - количество запросов за последние 5 минут. Каждые же 60 секунд сервис вычитает количетсов допустимых запросов.
	/// Если число запросов превышено - выдается бан.
	/// 
	/// Обновление: бан не выдается, как только число запросов вернется в рамки за период времени можноо будет отправить запрос снова
	/// </summary>
	public class RequestsFromIpCounterService : BackgroundService
	{
		public Dictionary<IPAddress, float> RequestsByIpLastTime { get; private set; } = new();
		private readonly AppSettingsProvider SettingsProvider;
		private const int UpdateDelay_mins = 1;

		private int MaxRequestsPerPeriod
			=> SettingsProvider.RequestsFromIpCounterServiceSP.AllowedNumOfRequestsPerMinute * SettingsProvider.RequestsFromIpCounterServiceSP.ControlledTime_mins;
		private int Period_mins
			=> SettingsProvider.RequestsFromIpCounterServiceSP.ControlledTime_mins;



		public RequestsFromIpCounterService(AppSettingsProvider SettingsProvider)
		{
			this.SettingsProvider = SettingsProvider;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (true)
			{
				Task.Delay(UpdateDelay_mins * 60 * 1000);
				/* Переменная содержит то, насколько будет умньшено число запросов на каждой итерации сервиса. 
				 * Вычисляется как (допустимое число запросов за период) * (период обновления/длинна периода).
				 * Таким образом, максимальное число запросов за период будет сниматься за один, собственно, период.
				 */
				float DecreaseRateInDelay = (float)this.MaxRequestsPerPeriod * ((float)UpdateDelay_mins / (float)Period_mins);
				foreach (var k in this.RequestsByIpLastTime.Keys)
				{
					RequestsByIpLastTime[k] = RequestsByIpLastTime[k] - DecreaseRateInDelay;
					if (RequestsByIpLastTime[k] < 0) RequestsByIpLastTime[k] = 0;
				}
			}
		}
		public void CountRequest(IPAddress RequesterIp)
		{
			if (!this.RequestsByIpLastTime.ContainsKey(RequesterIp))
				this.RequestsByIpLastTime.Add(RequesterIp, 1);
			else
				this.RequestsByIpLastTime[RequesterIp]++;
		}
		public void CountRequest(ActionExecutingContext context)
		{
			var ip = context.HttpContext.Connection.RemoteIpAddress;
			if (ip is not null)
				if (!this.RequestsByIpLastTime.ContainsKey(ip))
					this.RequestsByIpLastTime.Add(ip, 1);
		}
		public bool IPAddressIsBanned(IPAddress RequesterIp)
		{
			if (this.RequestsByIpLastTime.ContainsKey(RequesterIp))
				if (this.RequestsByIpLastTime[RequesterIp] > MaxRequestsPerPeriod) return true;

			return false;
		}
		public bool IPAddressIsBanned(ActionExecutingContext context)
		{
			if (context.HttpContext.Connection.RemoteIpAddress is not null)
				if (this.IPAddressIsBanned(context.HttpContext.Connection.RemoteIpAddress))
					return true;

			return false;
		}
	}
}