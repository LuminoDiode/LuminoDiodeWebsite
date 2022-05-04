﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;
using Website.Repository;

#nullable disable

namespace Website.Migrations
{
    [DbContext(typeof(WebsiteContext))]
    partial class WebsiteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Website.Models.DocumentModel.DbDocument", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AuthorId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<string[]>("Tags")
                        .HasColumnType("text[]");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<NpgsqlTsVector>("TitleTsVector")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasAnnotation("Npgsql:TsVectorConfig", "english")
                        .HasAnnotation("Npgsql:TsVectorProperties", new[] { "Title" });

                    b.Property<byte[]>("Utf8JsonSerializedParagraphs")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TitleTsVector");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("TitleTsVector"), "GIN");

                    b.ToTable("DbDocuments");
                });

            modelBuilder.Entity("Website.Models.ProjectModel.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectType")
                        .HasColumnType("integer");

                    b.Property<int?>("ProjectsGroupId")
                        .HasColumnType("integer");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ProjectsGroupId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Website.Models.ProjectsGroupModel.ProjectsGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("OwnerIdId")
                        .HasColumnType("integer");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OwnerIdId");

                    b.ToTable("ProjectsGroups");
                });

            modelBuilder.Entity("Website.Models.UserModel.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AboutMe")
                        .HasMaxLength(3000)
                        .HasColumnType("character varying(3000)");

                    b.Property<string>("AuthHashedPasswordString64")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AuthPasswordSaltString64")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("DisplayedName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("EmailAdress")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<int?>("ProjectsGroupId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("RegistrationCompleteDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("RegistrationStage")
                        .HasColumnType("integer");

                    b.Property<DateTime>("RegistrationStartedDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("String64_ProfileImage")
                        .HasColumnType("text");

                    b.Property<string>("TelegramLink")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("UserType")
                        .HasColumnType("integer");

                    b.Property<string>("VkLink")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasAlternateKey("EmailAdress");

                    b.HasIndex("ProjectId");

                    b.HasIndex("ProjectsGroupId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Website.Models.DocumentModel.DbDocument", b =>
                {
                    b.HasOne("Website.Models.UserModel.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("Website.Models.ProjectModel.Project", null)
                        .WithMany("OrderedDocumentsId")
                        .HasForeignKey("ProjectId");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Website.Models.ProjectModel.Project", b =>
                {
                    b.HasOne("Website.Models.UserModel.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Website.Models.ProjectsGroupModel.ProjectsGroup", null)
                        .WithMany("OrderedProjectsId")
                        .HasForeignKey("ProjectsGroupId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Website.Models.ProjectsGroupModel.ProjectsGroup", b =>
                {
                    b.HasOne("Website.Models.UserModel.User", "OwnerId")
                        .WithMany()
                        .HasForeignKey("OwnerIdId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OwnerId");
                });

            modelBuilder.Entity("Website.Models.UserModel.User", b =>
                {
                    b.HasOne("Website.Models.ProjectModel.Project", null)
                        .WithMany("Admins")
                        .HasForeignKey("ProjectId");

                    b.HasOne("Website.Models.ProjectsGroupModel.ProjectsGroup", null)
                        .WithMany("AdminsId")
                        .HasForeignKey("ProjectsGroupId");
                });

            modelBuilder.Entity("Website.Models.ProjectModel.Project", b =>
                {
                    b.Navigation("Admins");

                    b.Navigation("OrderedDocumentsId");
                });

            modelBuilder.Entity("Website.Models.ProjectsGroupModel.ProjectsGroup", b =>
                {
                    b.Navigation("AdminsId");

                    b.Navigation("OrderedProjectsId");
                });
#pragma warning restore 612, 618
        }
    }
}
