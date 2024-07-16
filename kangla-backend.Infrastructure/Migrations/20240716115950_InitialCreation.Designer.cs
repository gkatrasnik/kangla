﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(WateringContext))]
    [Migration("20240716115950_InitialCreation")]
    partial class InitialCreation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("Domain.Model.HumidityMeasurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("SoilHumidity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WateringDeviceId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WateringDeviceId");

                    b.ToTable("HumidityMeasurements");
                });

            modelBuilder.Entity("Domain.Model.WateringDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Deleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("DeviceToken")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastWatered")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("MinimumSoilHumidity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<bool>("WaterNow")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WateringDurationSetting")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WateringIntervalSetting")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("WateringDevices");
                });

            modelBuilder.Entity("Domain.Model.WateringEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("End")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Start")
                        .HasColumnType("TEXT");

                    b.Property<int>("WateringDeviceId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WateringDeviceId");

                    b.ToTable("WateringEvents");
                });

            modelBuilder.Entity("Domain.Model.HumidityMeasurement", b =>
                {
                    b.HasOne("Domain.Model.WateringDevice", "WateringDevice")
                        .WithMany("HumidityMeasurement")
                        .HasForeignKey("WateringDeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WateringDevice");
                });

            modelBuilder.Entity("Domain.Model.WateringEvent", b =>
                {
                    b.HasOne("Domain.Model.WateringDevice", "WateringDevice")
                        .WithMany("WateringEvents")
                        .HasForeignKey("WateringDeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WateringDevice");
                });

            modelBuilder.Entity("Domain.Model.WateringDevice", b =>
                {
                    b.Navigation("HumidityMeasurement");

                    b.Navigation("WateringEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
