﻿// <auto-generated />
using System;
using BuzzerWolf.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BuzzerWolf.Migrations
{
    [DbContext(typeof(BuzzerWolfContext))]
    [Migration("20241127051101_FixPrimaryKeys")]
    partial class FixPrimaryKeys
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("BuzzerWolf.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Divisions")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("FirstSeason")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("BuzzerWolf.Models.League", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CountryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DivisionLevel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("BuzzerWolf.Models.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AwayTeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AwayTeamScore")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HomeTeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("HomeTeamScore")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("WinningTeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("BuzzerWolf.Models.PlayoffSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("LeagueId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MatchId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("LeaguePlayoffs");
                });

            modelBuilder.Entity("BuzzerWolf.Models.Profile", b =>
                {
                    b.Property<int>("TeamId")
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

            modelBuilder.Entity("BuzzerWolf.Models.Result", b =>
                {
                    b.Property<int>("LeagueId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Winner")
                        .HasColumnType("INTEGER");

                    b.HasKey("LeagueId", "Season");

                    b.ToTable("LeagueResults");
                });

            modelBuilder.Entity("BuzzerWolf.Models.Season", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("Finish")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("Start")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("BuzzerWolf.Models.Standings", b =>
                {
                    b.Property<int>("LeagueId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Conference")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ConferenceRank")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsBot")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Losses")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PointsAgainst")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PointsFor")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("LeagueId", "Season", "TeamId");

                    b.ToTable("Standings");
                });

            modelBuilder.Entity("BuzzerWolf.Models.Sync", b =>
                {
                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DataTable")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("LastSync")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("NextAutoSync")
                        .HasColumnType("TEXT");

                    b.HasKey("TeamId", "DataTable");

                    b.ToTable("Sync");
                });
#pragma warning restore 612, 618
        }
    }
}
