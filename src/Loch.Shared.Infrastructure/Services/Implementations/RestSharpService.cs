using System.Threading.Tasks;
using Loch.Shared.Application.Dtos.Sms;
using Loch.Shared.Application.Services;
using RestSharp;
using RestSharp.Authenticators;

namespace Loch.Shared.Infrastructure.Services.Implementations
{
    public class RestSharpService : IRestSharpService
    {
        public async Task<bool> CreatePostRequest(string url, string body)
        {
            var client = new RestClient(url)
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            return response.IsSuccessful;
        }

        public async Task<bool> CreatePostRequest(string url, SmsRequestDto requestDto, string username, string password, string domain)
        {
            var client = new RestClient(url)
            {
                Authenticator = new HttpBasicAuthenticator(username + "/" + domain, password)
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("accept", "application/json");
            request.AddHeader("cache-control", "no-cache");

            request.AddParameter("senders", requestDto.Sender);
            request.AddParameter("recipients", requestDto.Receiver);
            request.AddParameter("messages", requestDto.Message);

            var response = await client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
    }
}
