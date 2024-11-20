using System.Net.Http;

namespace WebCon.ImportSubstitutionsApplication.Interfaces
{
    public interface IRestClient
    {
        void Send(string endpoint, string data);
        HttpResponseMessage Get(string endpoint);
    }
}
