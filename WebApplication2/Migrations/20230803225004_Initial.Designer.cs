﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication2.Data;

#nullable disable

namespace WebApplication2.Migrations
{
    [DbContext(typeof(LaptopStoreContext))]
    [Migration("20230803225004_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WebApplication2.Models.Brand", b =>
                {
                    b.Property<int>("BrandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BrandId"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BrandId");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("WebApplication2.Models.Laptop", b =>
                {
                    b.Property<Guid>("LaptopId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<int>("Condition")
                        .HasColumnType("int");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("LaptopId");

                    b.HasIndex("BrandId");

                    b.ToTable("Laptop");
                });

            modelBuilder.Entity("WebApplication2.Models.Laptop_Store", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StoreNumber")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("LaptopId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id", "StoreNumber", "LaptopId");

                    b.HasIndex("LaptopId");

                    b.HasIndex("StoreNumber");

                    b.ToTable("Laptop_Stores");
                });

            modelBuilder.Entity("WebApplication2.Models.StoreLocation", b =>
                {
                    b.Property<Guid>("StoreNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Province")
                        .HasColumnType("int");

                    b.Property<string>("StreetNameAndNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StoreNumber");

                    b.ToTable("StoreLocation");
                });

            modelBuilder.Entity("WebApplication2.Models.Laptop", b =>
                {
                    b.HasOne("WebApplication2.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");
                });

            modelBuilder.Entity("WebApplication2.Models.Laptop_Store", b =>
                {
                    b.HasOne("WebApplication2.Models.Laptop", "Laptop")
                        .WithMany("LaptopStores")
                        .HasForeignKey("LaptopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication2.Models.StoreLocation", "StoreLocation")
                        .WithMany("LaptopStores")
                        .HasForeignKey("StoreNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Laptop");

                    b.Navigation("StoreLocation");
                });

            modelBuilder.Entity("WebApplication2.Models.Laptop", b =>
                {
                    b.Navigation("LaptopStores");
                });

            modelBuilder.Entity("WebApplication2.Models.StoreLocation", b =>
                {
                    b.Navigation("LaptopStores");
                });
#pragma warning restore 612, 618
        }
    }
}
