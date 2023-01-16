namespace Loch.Shared.Application.Dtos.Sms
{
    public class SmsRequestDto
    {
       public string Sender { get; set; }
       public string Receiver { get; set; }
       public string Message { get; set; } 
    }
}