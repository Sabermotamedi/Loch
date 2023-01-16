using System.Threading.Tasks;
using Loch.Shared.Application.Dtos.Sms;

namespace Loch.Shared.Application.Services
{
    public interface IRestSharpService
    {
        Task<bool> CreatePostRequest(string url,string body);
        Task<bool> CreatePostRequest(string url, SmsRequestDto request, string username, string password, string domain);
   
    }
}