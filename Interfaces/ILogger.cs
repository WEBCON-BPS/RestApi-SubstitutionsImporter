namespace WebCon.ImportSubstitutionsApplication.Interfaces
{
    public interface ILogger
    {
        void Append(string message);
        void SaveLogs(string destinationPath);
    }
}
