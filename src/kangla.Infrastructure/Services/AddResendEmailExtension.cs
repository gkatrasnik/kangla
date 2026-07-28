using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resend;

namespace kangla.Infrastructure.Services
{
    public static class ResendEmailServiceExtensions
    {
        public static IServiceCollection AddResendEmail(this IServiceCollection services, IConfiguration configuration)
        {
            var resendApiKey = configuration["EmailSettings:ResendApiKey"]
                ?? Environment.GetEnvironmentVariable("EMAIL_SETTINGS_RESEND_API_KEY");

            if (string.IsNullOrWhiteSpace(resendApiKey))
            {
                throw new InvalidOperationException("EmailSettings:ResendApiKey is required.");
            }

            services.AddHttpClient(nameof(ResendClient));
            services.AddSingleton<IResend>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(ResendClient));
                var options = Options.Create(new ResendClientOptions { ApiToken = resendApiKey });

                return new ResendClient(new OptionsSnapshotWrapper<ResendClientOptions>(options), httpClient);
            });

            return services;
        }
    }

    internal sealed class OptionsSnapshotWrapper<T>(IOptions<T> options) : IOptionsSnapshot<T> where T : class
    {
        public T Value => options.Value;

        public T Get(string? name) => options.Value;
    }
}
