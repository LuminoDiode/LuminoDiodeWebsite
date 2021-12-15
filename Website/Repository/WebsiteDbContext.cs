﻿using Microsoft.EntityFrameworkCore;
using System.Linq;
using Website.Models;
using System;
using Utf8Json;
using RandomDataGenerator;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Update;
using NpgsqlTypes;
using Npgsql.Schema;

namespace Website.Repository
{
	public class WebsiteContext : DbContext
	{
		public WebsiteContext(DbContextOptions<WebsiteContext> options) : base(options)
		{
#if DEBUG
			Database.Migrate();
			//SeedData();
#endif
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Website.Models.DocumentModel.DbDocument>().
				HasGeneratedTsVectorColumn(p => p.TitleTsVector,"english", p => new { p.Title }) 
				.HasIndex(p => p.TitleTsVector)
				.HasMethod("GIN");
		}

		public DbSet<Website.Models.DocumentModel.DbDocument> DbDocuments { get; set; }
		public DbSet<Website.Models.UserModel.User> Users { get; set; }
		public DbSet<Website.Models.ProjectModel.Project> Projects { get; set; }
		public DbSet<Website.Models.ProjectsGroupModel.ProjectsGroup> ProjectsGroups { get; set; }

#if DEBUG
		private void SeedData(bool SaveChanges = true)
		{
			SeedData_DbDocuments();
			SeedData_Users();
			SeedData_Projects();
		}

		private void SeedData_DbDocuments(bool DoNotSeedIfDataExists = false)
		{
			// UNSAFE!
			// Some SQL DBs, like SQL Server creates non-linear ids, it means that there can be ids like 1,2,1001,1003,9991 in the db with only 5 added items,
			// so this is kinda unstable code, but should work with postgres

			// this checks if there is at least 10 raws in the db
			//if (this.DbDocuments.Find(10000) != null && DoNotSeedIfDataExists) return;

			var docsToAdd = new object[10000].Select(x => Models.DocumentModel.DbDocument.FromDocument(Website.Models.DocumentModel.Document.GenerateRandom()));
			this.DbDocuments.AddRange(docsToAdd);
			this.SaveChanges();
		}
		private void SeedData_Users()
		{
			var ToAdd = new Models.UserModel.User
			{
				Id = null,
				FirstName = "Admin",
				LastName = "Adminovich"
			};
			this.Users.Add(ToAdd);
			this.SaveChanges();
		}
		private void SeedData_Projects()
		{
			var ToAdd = new object[100].Select(x => Website.Models.ProjectModel.Project.GenerateRandom(this));
			this.Projects.AddRange(ToAdd);
			this.SaveChanges();
		}
		private void SeedData_ProjectsGroups()
		{

		}
#endif

	}
}
