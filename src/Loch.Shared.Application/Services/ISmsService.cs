using System.Threading.Tasks;
using Loch.Shared.Application.Dtos.Sms;

namespace Loch.Shared.Application.Services;

    public interface ISmsService
    {
        Task<bool> SendMessage(SmsRequestDto requestDto);
        SmsRequestDto CreateSendRequest(string receiver,string message);    
    }

