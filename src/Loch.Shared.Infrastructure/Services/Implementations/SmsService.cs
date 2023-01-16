using System;
using System.Threading.Tasks;
using Loch.Shared.Application.Dtos.Sms;
using Loch.Shared.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Loch.Shared.Infrastructure.Services.Implementations
{
    public class SmsService : ISmsService
    {
       
        private readonly IRestSharpService _restSharpService;
        private readonly IConfiguration _configuration;

        public SmsService(IRestSharpService restSharpService, IConfiguration configuration)
        {
            _restSharpService = restSharpService;
            _configuration = configuration;
        }


        public async Task<bool> SendMessage(SmsRequestDto request)
        {
            var url = _configuration.GetSection("Magfa").GetSection("SendUrl").Value;
            var username = _configuration.GetSection("Magfa").GetSection("Username").Value;
            var password = _configuration.GetSection("Magfa").GetSection("Password").Value;
            var domain = _configuration.GetSection("Magfa").GetSection("Domain").Value;

            return await _restSharpService.CreatePostRequest(url, request, username, password, domain);
        }

        public SmsRequestDto CreateSendRequest(string receiver, string message)
        {
            var sender = _configuration.GetSection("Magfa").GetSection("Sender").Value;
            var data = new SmsRequestDto
            {
                Message = message,
                Receiver = receiver,
                Sender = sender
            };
            return data;
        }
    }
}
