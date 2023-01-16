namespace Loch.Shared.Core.Application
{
    public interface IMessage
    {
        Dictionary<long, string> Messages { get; }
    }
}