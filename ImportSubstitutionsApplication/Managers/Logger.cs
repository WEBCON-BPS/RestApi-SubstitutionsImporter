using WebCon.ImportSubstitutionsApplication.Interfaces;
using System.IO;
using System.Text;

namespace WebCon.ImportSubstitutionsApplication.Managers
{
    public class Logger : ILogger
    {
        private readonly StringBuilder _logger = new StringBuilder();

        public void Append(string message)
        {
            _logger.AppendLine(message);
        }

        public void SaveLogs(string destinationPath)
        {
            using (var streamWriter = GetStreamWriter(destinationPath))
            {
                streamWriter.WriteLine(_logger.ToString());
            }
        }

        private StreamWriter GetStreamWriter(string filePath)
        {
            return File.Exists(filePath) ? File.AppendText(filePath) : File.CreateText(filePath);
        }
    }
}
