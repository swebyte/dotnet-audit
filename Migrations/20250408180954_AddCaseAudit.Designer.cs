﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CaseApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250408180954_AddCaseAudit")]
    partial class AddCaseAudit
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("Case", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Cases");
                });

            modelBuilder.Entity("CaseAudit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Action")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EntityId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CaseAudits");
                });

            modelBuilder.Entity("CaseTypeA", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CaseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExtraA")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CaseId")
                        .IsUnique();

                    b.ToTable("CaseTypeAs");
                });

            modelBuilder.Entity("CaseTypeB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CaseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExtraB")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CaseId")
                        .IsUnique();

                    b.ToTable("CaseTypeBs");
                });

            modelBuilder.Entity("CaseTypeC", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CaseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExtraC")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CaseId")
                        .IsUnique();

                    b.ToTable("CaseTypeCs");
                });

            modelBuilder.Entity("CaseTypeA", b =>
                {
                    b.HasOne("Case", null)
                        .WithOne("CaseTypeA")
                        .HasForeignKey("CaseTypeA", "CaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CaseTypeB", b =>
                {
                    b.HasOne("Case", null)
                        .WithOne("CaseTypeB")
                        .HasForeignKey("CaseTypeB", "CaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CaseTypeC", b =>
                {
                    b.HasOne("Case", null)
                        .WithOne("CaseTypeC")
                        .HasForeignKey("CaseTypeC", "CaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Case", b =>
                {
                    b.Navigation("CaseTypeA")
                        .IsRequired();

                    b.Navigation("CaseTypeB")
                        .IsRequired();

                    b.Navigation("CaseTypeC")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
