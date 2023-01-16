using System.Collections.Generic;

namespace Loch.Shared.Application.Dtos.Token
{
    public record UserPermissionsDto
    {
        public List<long> RoleIds { get; set; }
        public List<long> GroupIds { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> PermissionGroups { get; set; }
    }
}
