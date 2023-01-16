using MediatR;
using Loch.Shared.Core.Application;
using Loch.Shared.Models;

namespace Loch.Shared.Commands
{
    public abstract class Command : IRequest<AppResult>
    {
        public UserInfo CurrentUserInfo { get; set; }
        public string SignData { get; set; }
    }
}
