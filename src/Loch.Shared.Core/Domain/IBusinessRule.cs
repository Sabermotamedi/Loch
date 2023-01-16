namespace Loch.Shared.Core.Domain
{
    public interface IBusinessRule
    {
        bool IsBroken();

        Error Error { get; }
    }
}