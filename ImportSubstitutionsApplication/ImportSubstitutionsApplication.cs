using WebCon.ImportSubstitutionsApplication.Interfaces;
using WebCon.ImportSubstitutionsApplication.Managers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebCon.ImportSubstitutionsApplication
{
    public class ImportSubstitutionsApplication
    {
        private readonly ISubstitutionsManager _substitutionsManager;
        private readonly IConfigurationSettings _configurationSettings;
        private readonly ILogger _logger;

        public ImportSubstitutionsApplication(ISubstitutionsManager externalSubstitutionsManager, ILogger logger, IConfigurationSettings configurationSettings)
        {
            _substitutionsManager = externalSubstitutionsManager;
            _logger = logger;
            _configurationSettings = configurationSettings;
        }

        static void Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                host.Services.GetRequiredService<ImportSubstitutionsApplication>().ImportSubstitutions();
            }
        }

        private void ImportSubstitutions()
        {
            try
            {
                _logger.Append($"------ Import started at {DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff} ------");
                _substitutionsManager.ImportSubstitutions();
                _logger.Append($"------ Import ended at {DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff} ------");
            }
            catch (Exception e)
            {
                _logger.Append(e.ToString());
            }
            finally
            {
                _logger.SaveLogs(_configurationSettings.LogsFilePath);
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ImportSubstitutionsApplication>();
                    services.AddSingleton<ISubstitutionsManager, SubstitutionsManager>();
                    services.AddSingleton<IConfigurationSettings, ConfigurationSettings>();
                    services.AddSingleton<IRestClient, RestClient>();
                    services.AddSingleton<ILogger, Logger>();
                });
        }
    }
}
