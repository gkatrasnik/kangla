﻿using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;
using System.Net;
using kangla.Domain.Interfaces;
using kangla.Infrastructure.Repositories;
using kangla.Infrastructure.Services;
using kangla.Infrastructure;

namespace kangla.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            services.AddDbContext<PlantsContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("PlantsContextSQLite")));

            services.AddTransient<DatabaseSeeder>();
            services.AddTransient<IDatabaseMigrationService, DatabaseMigrationService>();
            services.AddScoped<IWateringDeviceRepository, WateringDeviceRepository>();
            services.AddScoped<IWateringEventRepository, WateringEventRepository>();
            services.AddScoped<IHumidityMeasurementRepository, HumidityMeasurementRepository>();
            services.AddScoped<IPlantsRepository, PlantsRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddFluentEmail(configuration);
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IImageProcessingService, ImageProcessingService>();
            services.AddHttpClient<IPlantRecognitionService, PlantRecognitionService>();
            return services;
        }


        public static void AddFluentEmail(this IServiceCollection services,
            IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            var defaultFromEmail = emailSettings["DefaultFromEmail"] ?? Environment.GetEnvironmentVariable("EMAIL_SETTINGS_DEFAULT_FROM_EMAIL");
            var host = emailSettings["Host"] ?? Environment.GetEnvironmentVariable("EMAIL_SETTINGS_HOST");
            var portString = emailSettings["Port"] ?? Environment.GetEnvironmentVariable("EMAIL_SETTINGS_PORT");
            var username = emailSettings["Username"] ?? Environment.GetEnvironmentVariable("EMAIL_SETTINGS_USERNAME");
            var password = emailSettings["Password"] ?? Environment.GetEnvironmentVariable("EMAIL_SETTINGS_PASSWORD");

            if (!int.TryParse(portString, out var port))
            {
                throw new ArgumentException("Invalid or missing port number.");
            }

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            services.AddFluentEmail(defaultFromEmail);
            services.AddSingleton<ISender>(x => new SmtpSender(smtpClient));
        }
    }
}
