﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaystackIntegrationPracticeApi.Data;

#nullable disable

namespace PaystackIntegrationPracticeApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241016142440_AddColumnsForTransactionVerificationSource")]
    partial class AddColumnsForTransactionVerificationSource
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.35");

            modelBuilder.Entity("PaystackIntegrationPracticeApi.Models.PaystackIntegration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AccessCode")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("AmountInBaseDenomination")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IntializeTransactionResponseMessage")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.Property<string>("TransactionVerificationMessage")
                        .HasColumnType("TEXT");

                    b.Property<bool?>("TransactionVerified")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("VerificationMeansUsed")
                        .HasColumnType("TEXT");

                    b.Property<string>("VerifiedTransactionStatus")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PaystackIntegration");
                });
#pragma warning restore 612, 618
        }
    }
}
