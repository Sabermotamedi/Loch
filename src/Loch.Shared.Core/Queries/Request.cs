using MediatR;
using Loch.Shared.Core.Application;
using Loch.Shared.Models;

namespace Loch.Shared.Queriess
{
    public class Request : IRequest<AppResult>
    {
        public UserInfo CurrentUserInfo { get; set; }
    }
}
