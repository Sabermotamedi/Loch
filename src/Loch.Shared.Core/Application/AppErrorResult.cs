namespace Loch.Shared.Core.Application
{
    public class AppErrorResult
    {
        public AppErrorResult(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; }
        public string Message { get; }
    }
}