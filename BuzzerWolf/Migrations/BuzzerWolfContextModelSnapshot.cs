﻿// <auto-generated />
using BuzzerWolf.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BuzzerWolf.Migrations
{
    [DbContext(typeof(BuzzerWolfContext))]
    partial class BuzzerWolfContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("BuzzerWolf.Models.Profile", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccessKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("SecondTeam")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TeamId");

                    b.ToTable("Profiles");
                });
#pragma warning restore 612, 618
        }
    }
}
