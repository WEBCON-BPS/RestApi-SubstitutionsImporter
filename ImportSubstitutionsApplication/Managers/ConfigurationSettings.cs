using System;
using System.Configuration;
using WebCon.ImportSubstitutionsApplication.Interfaces;

namespace WebCon.ImportSubstitutionsApplication.Managers
{
    public class ConfigurationSettings : IConfigurationSettings
    {
        public string ConnectionString { get; }
        public string SqlCommand { get; }
        public string PortalUrl { get; }
        public int DatabaseId { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string LogsFilePath { get; }

        public ConfigurationSettings()
        {
            try
            {
                ConnectionString = ConfigurationManager.AppSettings.Get(nameof(ConnectionString)) ??
                                   throw new ArgumentException($"{nameof(ConnectionString)} must be defined in App.config file.");
                SqlCommand = ConfigurationManager.AppSettings.Get(nameof(SqlCommand)) ??
                             throw new ArgumentException($"{nameof(SqlCommand)} must be defined in App.config file.");
                PortalUrl = ConfigurationManager.AppSettings.Get(nameof(PortalUrl)) ??
                            throw new ArgumentException($"{nameof(PortalUrl)} must be defined in App.config file.");
                DatabaseId = int.Parse(ConfigurationManager.AppSettings.Get(nameof(DatabaseId)) ??
                             throw new ArgumentException($"{nameof(DatabaseId)} must be defined in App.config file."));
                ClientId = ConfigurationManager.AppSettings.Get(nameof(ClientId)) ??
                           throw new ArgumentException($"{nameof(ClientId)} must be defined in App.config file.");
                ClientSecret = ConfigurationManager.AppSettings.Get(nameof(ClientSecret)) ??
                               throw new ArgumentException($"{nameof(ClientSecret)} must be defined in App.config file.");
                LogsFilePath = ConfigurationManager.AppSettings.Get(nameof(LogsFilePath)) ??
                               throw new ArgumentException($"{nameof(LogsFilePath)} must be defined in App.config file.");
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid configuration in App.config file. Make sure that all configuration properties are properly set.");
            }
        }
    }
}
