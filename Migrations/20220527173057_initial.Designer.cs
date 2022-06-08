﻿// <auto-generated />
using System;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(SWPPDbContext))]
    [Migration("20220527173057_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Backend.Models.BankingInfo", b =>
                {
                    b.Property<Guid>("BankId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BankLocation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankWebsite")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("BankId");

                    b.HasIndex("ProfileNumber")
                        .IsUnique();

                    b.ToTable("BankingInfos");

                    b.HasData(
                        new
                        {
                            BankId = new Guid("40822906-84f0-4dae-b8f0-696fce457db8"),
                            BankLocation = "Dong Da, Ha Noi, Ngan Hang co phan quan doi MB Bank, Merchant",
                            BankWebsite = "www.merchant.com",
                            Name = "Merchant Duy",
                            ProfileNumber = "7467811997849"
                        },
                        new
                        {
                            BankId = new Guid("dbe552e4-37de-4ffb-b920-ab7caa9ebe0d"),
                            BankLocation = "Dong Da, Ha Noi, Ngan Hang co phan quan doi MB Bank, Client",
                            BankWebsite = "www.client.com",
                            Name = "Client Duy",
                            ProfileNumber = "1255070770448"
                        });
                });

            modelBuilder.Entity("Backend.Repository.Entity.BankAccount", b =>
                {
                    b.Property<string>("ProfileNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Balances")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProfileNumber");

                    b.ToTable("BankAccounts");

                    b.HasData(
                        new
                        {
                            ProfileNumber = "7467811997849",
                            Balances = 999,
                            Name = "Merchant"
                        },
                        new
                        {
                            ProfileNumber = "1255070770448",
                            Balances = 999,
                            Name = "Client"
                        });
                });

            modelBuilder.Entity("Backend.Repository.Entity.SessionKeys", b =>
                {
                    b.Property<Guid>("SessionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ClientWriteKeyBase64")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientWriteMacKeyBase64")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServerWriteKeyBase64")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServerWriteMacKeyBase64")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SessionID");

                    b.ToTable("SessionKeys");
                });

            modelBuilder.Entity("Backend.Repository.Entity.Transaction", b =>
                {
                    b.Property<Guid>("TransactionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<string>("Payee")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Payer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TransactionID");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Backend.Models.BankingInfo", b =>
                {
                    b.HasOne("Backend.Repository.Entity.BankAccount", null)
                        .WithOne()
                        .HasForeignKey("Backend.Models.BankingInfo", "ProfileNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}