﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Website.Repository;
using Website.Services;

namespace Website.Controllers
{
	public class DocumentController : Controller
	{
		private readonly IServiceScopeFactory ScopeFactory;
		private readonly Website.Repository.WebsiteContext context;
		private readonly Website.Services.RecentDocumentsBackgroundService recentDocumentsProvider;
		public DocumentController(IServiceScopeFactory Services, Website.Services.RecentDocumentsBackgroundService documentsBackgroundService)
		{
			this.ScopeFactory = Services;
			this.context = Services.CreateScope().ServiceProvider.GetRequiredService<WebsiteContext>();
			this.recentDocumentsProvider = documentsBackgroundService;
		}

		[HttpGet]
		public ViewResult Summary()
		{
			var rd = this.recentDocumentsProvider.RecentDocuments;

			Website.Models.DocumentWithAuthorStruct[] documentWithAuthorStructs = new Models.DocumentWithAuthorStruct[3]; int i = 0;

			foreach (var doc in rd)
			{
				documentWithAuthorStructs[i++] = new Website.Models.DocumentWithAuthorStruct { Document = doc.ToDocument(), AuthorUser = doc.Author };
				if (i == 3) break;
			}

			return this.View(documentWithAuthorStructs);
		}

		[HttpGet]
		public IActionResult Show(int Id)
		{
			var loadedDoc = this.context.DbDocuments.Include("Author").First();
			if (loadedDoc == null) return new StatusCodeResult(404);

			return this.View(loadedDoc);
		}

		[HttpPost]
		public ViewResult Search(string SearchRequest)
		{
			this.ViewBag.SearchRequest = SearchRequest;

			var ServiceProvider = this.ScopeFactory.CreateScope().ServiceProvider;
			var SearchService = ServiceProvider.GetRequiredService<DocumentSearchService>();
			var ProceedRes = SearchService.ProceedRequest(SearchRequest);
			var ResResult = ProceedRes;
			var Loaded = ResResult.Take(10).ToList().Select(x => new Models.DocumentWithAuthorStruct
			{
				Document = x.ToDocument(),
				AuthorUser = x.Author
			});
			return this.View(Loaded);
		}

		#region Create
		[HttpGet]
		public ViewResult Create()
		{
			return this.View();
		}

		[HttpPost]
		public StatusCodeResult Create(object DocumentPassedForCreation/*tobedefinied*/)
		{
			/* HTTP Status 202 indicates that the request 
			 * has been accepted for processing, 
			 * but the processing has not been completed.
			 */
			return new StatusCodeResult(202);
		}
		#endregion

		#region Edit
		[HttpGet]
		public ViewResult Edit(int ProjectId)
		{
			return this.View();
		}

		[HttpPut]
		public StatusCodeResult Edit(int ProjectId, object DocumentToBePosted)
		{
			return new StatusCodeResult(202);
		}
		#endregion

		#region Delete
		[HttpGet]
		public ViewResult Delete(int ProjectId)
		{
			return this.View();
		}
		[HttpDelete]
		public StatusCodeResult Delete(int ProjectId, int UserPerformsActionId)
		{
			return new StatusCodeResult(202);
		}
		#endregion



	}
}
