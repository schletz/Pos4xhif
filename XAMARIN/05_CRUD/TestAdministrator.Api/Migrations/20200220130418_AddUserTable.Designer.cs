﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestAdministrator.Api.Model;

namespace TestAdministrator.Api.Migrations
{
    [DbContext(typeof(TestsContext))]
    [Migration("20200220130418_AddUserTable")]
    partial class AddUserTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0");

            modelBuilder.Entity("TestAdministrator.Api.Model.Lesson", b =>
                {
                    b.Property<long>("L_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("L_Class")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)");

                    b.Property<long?>("L_Day")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValueSql("0");

                    b.Property<long?>("L_Hour")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValueSql("0");

                    b.Property<string>("L_Room")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("L_Subject")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("L_Teacher")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)");

                    b.Property<long?>("L_Untis_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValueSql("0");

                    b.HasKey("L_ID");

                    b.HasIndex("L_Class")
                        .HasName("SchoolclassLesson");

                    b.HasIndex("L_Hour")
                        .HasName("PeriodLesson");

                    b.HasIndex("L_Teacher")
                        .HasName("TeacherLesson");

                    b.HasIndex("L_Untis_ID")
                        .HasName("idx_L_Untis_ID");

                    b.ToTable("Lesson");
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Period", b =>
                {
                    b.Property<long>("P_Nr")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("P_From")
                        .HasColumnType("TIMESTAMP");

                    b.Property<DateTime>("P_To")
                        .HasColumnType("TIMESTAMP");

                    b.HasKey("P_Nr");

                    b.ToTable("Period");
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Pupil", b =>
                {
                    b.Property<long>("P_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("P_Account")
                        .IsRequired()
                        .HasColumnType("VARCHAR(16)");

                    b.Property<string>("P_Class")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("P_Firstname")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("P_Lastname")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("P_ID");

                    b.HasIndex("P_Account")
                        .IsUnique()
                        .HasName("idx_P_Account");

                    b.HasIndex("P_Class")
                        .HasName("SchoolclassPupil");

                    b.ToTable("Pupil");
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Schoolclass", b =>
                {
                    b.Property<string>("C_ID")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("C_ClassTeacher")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("C_Department")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)");

                    b.HasKey("C_ID");

                    b.HasIndex("C_ClassTeacher")
                        .HasName("TeacherSchoolclass");

                    b.ToTable("Schoolclass");
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Teacher", b =>
                {
                    b.Property<string>("T_ID")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("T_Account")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("T_Email")
                        .HasColumnType("VARCHAR(255)");

                    b.Property<string>("T_Firstname")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("T_Lastname")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("T_ID");

                    b.HasIndex("T_Account")
                        .IsUnique()
                        .HasName("idx_T_Account");

                    b.ToTable("Teacher");
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Test", b =>
                {
                    b.Property<long>("TE_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("TE_Class")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)");

                    b.Property<DateTime>("TE_Date")
                        .HasColumnType("TIMESTAMP");

                    b.Property<long>("TE_Lesson")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TE_Subject")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)");

                    b.Property<string>("TE_Teacher")
                        .IsRequired()
                        .HasColumnType("VARCHAR(8)");

                    b.HasKey("TE_ID");

                    b.HasIndex("TE_Class")
                        .HasName("SchoolclassTest");

                    b.HasIndex("TE_Lesson")
                        .HasName("PeriodTest");

                    b.HasIndex("TE_Teacher")
                        .HasName("TeacherTest");

                    b.ToTable("Test");
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.User", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnName("U_Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.Property<string>("Hash")
                        .HasColumnName("U_Hash")
                        .HasColumnType("TEXT")
                        .HasMaxLength(44);

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnName("U_LastLogin")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnName("U_Salt")
                        .HasColumnType("TEXT")
                        .HasMaxLength(24);

                    b.HasKey("Name");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Lesson", b =>
                {
                    b.HasOne("TestAdministrator.Api.Model.Schoolclass", "L_ClassNavigation")
                        .WithMany("Lesson")
                        .HasForeignKey("L_Class")
                        .IsRequired();

                    b.HasOne("TestAdministrator.Api.Model.Period", "L_HourNavigation")
                        .WithMany("Lesson")
                        .HasForeignKey("L_Hour");

                    b.HasOne("TestAdministrator.Api.Model.Teacher", "L_TeacherNavigation")
                        .WithMany("Lesson")
                        .HasForeignKey("L_Teacher")
                        .IsRequired();
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Pupil", b =>
                {
                    b.HasOne("TestAdministrator.Api.Model.Schoolclass", "P_ClassNavigation")
                        .WithMany("Pupil")
                        .HasForeignKey("P_Class")
                        .IsRequired();
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Schoolclass", b =>
                {
                    b.HasOne("TestAdministrator.Api.Model.Teacher", "C_ClassTeacherNavigation")
                        .WithMany("Schoolclass")
                        .HasForeignKey("C_ClassTeacher");
                });

            modelBuilder.Entity("TestAdministrator.Api.Model.Test", b =>
                {
                    b.HasOne("TestAdministrator.Api.Model.Schoolclass", "TE_ClassNavigation")
                        .WithMany("Test")
                        .HasForeignKey("TE_Class")
                        .IsRequired();

                    b.HasOne("TestAdministrator.Api.Model.Period", "TE_LessonNavigation")
                        .WithMany("Test")
                        .HasForeignKey("TE_Lesson")
                        .IsRequired();

                    b.HasOne("TestAdministrator.Api.Model.Teacher", "TE_TeacherNavigation")
                        .WithMany("Test")
                        .HasForeignKey("TE_Teacher")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}