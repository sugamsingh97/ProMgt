﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProMgt.Data;

#nullable disable

namespace ProMgt.Migrations.ProjectDb
{
    [DbContext(typeof(ProjectDbContext))]
    partial class ProjectDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("ProMgt.Data.Model.Priority", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ColorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ColorId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Priorities");
                });

            modelBuilder.Entity("ProMgt.Data.Model.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfCreation")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeadLine")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProjectStatusId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectStatusId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("ProMgt.Data.Model.ProjectMgtColor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("HexCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ProjectMgtColors");
                });

            modelBuilder.Entity("ProMgt.Data.Model.ProjectStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("HexCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ProjectStatuses");
                });

            modelBuilder.Entity("ProMgt.Data.Model.ProjectTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfCreation")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeadLine")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("PriorityId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SectionId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TaskStatusId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PriorityId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SectionId");

                    b.HasIndex("TaskStatusId");

                    b.ToTable("ProjectTasks");
                });

            modelBuilder.Entity("ProMgt.Data.Model.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("ProMgt.Data.Model.TaskStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ColorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ColorId");

                    b.HasIndex("ProjectId");

                    b.ToTable("TaskStatuses");
                });

            modelBuilder.Entity("ProMgt.Data.Model.Priority", b =>
                {
                    b.HasOne("ProMgt.Data.Model.ProjectMgtColor", "Color")
                        .WithMany("Priorities")
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ProMgt.Data.Model.Project", "Project")
                        .WithMany("Priorities")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Color");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ProMgt.Data.Model.Project", b =>
                {
                    b.HasOne("ProMgt.Data.Model.ProjectStatus", "ProjectStatus")
                        .WithMany("Projects")
                        .HasForeignKey("ProjectStatusId");

                    b.Navigation("ProjectStatus");
                });

            modelBuilder.Entity("ProMgt.Data.Model.ProjectTask", b =>
                {
                    b.HasOne("ProMgt.Data.Model.Priority", "Priority")
                        .WithMany("Tasks")
                        .HasForeignKey("PriorityId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("ProMgt.Data.Model.Project", "Project")
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProMgt.Data.Model.Section", "Section")
                        .WithMany("Tasks")
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("ProMgt.Data.Model.TaskStatus", "TaskStatus")
                        .WithMany("Tasks")
                        .HasForeignKey("TaskStatusId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Priority");

                    b.Navigation("Project");

                    b.Navigation("Section");

                    b.Navigation("TaskStatus");
                });

            modelBuilder.Entity("ProMgt.Data.Model.Section", b =>
                {
                    b.HasOne("ProMgt.Data.Model.Project", "Project")
                        .WithMany("Sections")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ProMgt.Data.Model.TaskStatus", b =>
                {
                    b.HasOne("ProMgt.Data.Model.ProjectMgtColor", "Color")
                        .WithMany("TaskStatuses")
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ProMgt.Data.Model.Project", "Project")
                        .WithMany("TaskStatuses")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Color");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ProMgt.Data.Model.Priority", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("ProMgt.Data.Model.Project", b =>
                {
                    b.Navigation("Priorities");

                    b.Navigation("Sections");

                    b.Navigation("TaskStatuses");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("ProMgt.Data.Model.ProjectMgtColor", b =>
                {
                    b.Navigation("Priorities");

                    b.Navigation("TaskStatuses");
                });

            modelBuilder.Entity("ProMgt.Data.Model.ProjectStatus", b =>
                {
                    b.Navigation("Projects");
                });

            modelBuilder.Entity("ProMgt.Data.Model.Section", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("ProMgt.Data.Model.TaskStatus", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
