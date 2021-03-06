using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Website.Repository;
using Website.Services;

namespace Website.Controllers
{
	/// <summary>
	/// Literally 'my controller'. A class that should be inherited by all controllers on this website.
	/// </summary>
	public abstract class AMyController : Controller
	{
		protected readonly IServiceProvider ServiceProvider;
		protected readonly WebsiteContext context;
		protected readonly SessionManager SM;
		protected readonly RequestsFromIpCounterService RC;

		protected Website.Models.UserModel.User? AuthedUser { get; set; } = null;

		protected const string PageTopAlertsTempDataName = "PageTopAlerts";
		//protected readonly List<Website.Models.ViewModels.Alert> PageTopAlerts = new(1);

		public AMyController(IServiceScopeFactory ScopeFactory)
		{
			var sp = ScopeFactory.CreateScope().ServiceProvider;
			this.ServiceProvider = sp;
			this.context = sp.GetRequiredService<WebsiteContext>();
			this.SM = sp.GetRequiredService<SessionManager>();
			this.RC = sp.GetRequiredService<RequestsFromIpCounterService>();
			this.ViewBag.AuthedUser = new Website.Models.UserModel.User { };
			this.ViewBag.AuthedUser = null;
		}
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			// Returns Http 429 if too many requests
			this.RC.CountRequest(context);
			if (this.RC.IPAddressIsBanned(context)) context.Result = new StatusCodeResult(429);

			base.OnActionExecuting(context);

			this.LoadSessionAndAuthedUser();
			this.RedirectToRegistrationStepIfNeeded(context);
		}
		private void LoadSessionAndAuthedUser()
		{
			if (this.Request.Cookies.ContainsKey(SessionManager.SessionIdCoockieName))
			{
				SessionInfo? Info=null;
				if(this.Request.Cookies.TryGetValue(SessionManager.SessionIdCoockieName, out var cookieVal))
					this.SM.ValidateSession(cookieVal!, out Info);

				if (Info is null) return;

				this.AuthedUser = this.context.Users.AsTracking(QueryTrackingBehavior.TrackAll).First(u => u.Id == Info.UserId); /* throw new System.AggregateException("Session cotains user id which cannot be found in DB")*/;
				this.ViewBag.AuthedUser = this.AuthedUser;
			}
		}

		protected void AddAlertToPageTop(Website.Models.ViewModels.Alert alert)
		{
			if (!this.TempData.ContainsKey(PageTopAlertsTempDataName))
			{
				this.TempData.Add(PageTopAlertsTempDataName, string.Empty);
			}
			if (this.TempData.Peek(PageTopAlertsTempDataName) is not string)
			{
				this.TempData.Remove(PageTopAlertsTempDataName);
				this.TempData.Add(PageTopAlertsTempDataName, string.Empty);
			}

			var peekedStr = (string)this.TempData.Peek(PageTopAlertsTempDataName)!;
			var StringWithNewAlert = peekedStr + (string.IsNullOrEmpty(peekedStr) ? string.Empty : "\n") + alert.ToHtmlString();

			this.TempData.Remove(PageTopAlertsTempDataName);
			this.TempData.Add(PageTopAlertsTempDataName, StringWithNewAlert);
		}

		private enum ForwardingTo
		{
			LogoutPage,
			EnteringNamePage,
			EnteringMetadataPage,
			OtherPages
		}
		private void RedirectToRegistrationStepIfNeeded(ActionExecutingContext context)
		{
			if (this.AuthedUser == null) return; // RETURN; if no user authed

			var act = context.RouteData.Values["Action"] ?? string.Empty;
			var ctr = context.RouteData.Values["controller"] ?? string.Empty;

			ForwardingTo CurrentDist = ForwardingTo.OtherPages;

			var UserCtrName = nameof(Website.Controllers.UserController).Replace("Controller", string.Empty);
			var LogoutActName = nameof(Website.Controllers.UserController.Logout);
			var EntNameActName = nameof(Website.Controllers.UserController.RegistrationStageEnteringName);
			var EntMetaActName = nameof(Website.Controllers.UserController.RegistrationStageEnteringMetadata);

			if (ctr.Equals(UserCtrName))
			{
				if (act.Equals(LogoutActName))
					CurrentDist = ForwardingTo.LogoutPage; // could return here, but returning just anywhere is not good
				else if (act.Equals(EntNameActName))
					CurrentDist = ForwardingTo.EnteringNamePage;
				else if (act.Equals(EntMetaActName))
					CurrentDist = ForwardingTo.EnteringMetadataPage;
			}

			if (CurrentDist == ForwardingTo.LogoutPage) return; // RETURN; If trying to logout, let him

			ForwardingTo NeededDist = ForwardingTo.OtherPages;

			if (this.AuthedUser.RegistrationStage == Models.UserModel.User.REGISTRATION_STAGE.EnteringName)
				NeededDist = ForwardingTo.EnteringNamePage;
			else if (this.AuthedUser.RegistrationStage == Models.UserModel.User.REGISTRATION_STAGE.EnteringMetadata)
				NeededDist = ForwardingTo.EnteringMetadataPage;

			if (NeededDist == CurrentDist) return; // RETURN; already heading needed page

			if (NeededDist == ForwardingTo.EnteringNamePage)
				context.Result = this.RedirectToAction(EntNameActName, UserCtrName);
			else if (NeededDist == ForwardingTo.EnteringMetadataPage)
				context.Result = this.RedirectToAction(EntMetaActName, UserCtrName);
		}
	}
}