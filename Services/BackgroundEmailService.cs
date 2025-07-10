using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Admin.Services;

namespace Admin.Services
{
    public class BackgroundEmailService : BackgroundService
    {
        private readonly ILogger<BackgroundEmailService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public BackgroundEmailService(
            ILogger<BackgroundEmailService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Email Service started");

            // Get processing interval from configuration (default: 5 minutes)
            var processingIntervalMinutes = int.Parse(_configuration["Email:ProcessingIntervalMinutes"] ?? "5");
            var processingInterval = TimeSpan.FromMinutes(processingIntervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        await emailService.ProcessEmailQueueAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing email queue in background service");
                }

                try
                {
                    await Task.Delay(processingInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Expected when cancellation token is triggered
                    break;
                }
            }

            _logger.LogInformation("Background Email Service stopped");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Email Service is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
} 