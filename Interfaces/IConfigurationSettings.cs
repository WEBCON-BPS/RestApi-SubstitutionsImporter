namespace WebCon.ImportSubstitutionsApplication.Interfaces
{
    public interface IConfigurationSettings
    {
        string ConnectionString { get; }
        string SqlCommand { get; }
        string PortalUrl { get; }
        int DatabaseId { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string LogsFilePath { get; }
    }
}
