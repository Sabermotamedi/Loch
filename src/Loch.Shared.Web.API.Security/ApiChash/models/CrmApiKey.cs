using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash
{
    public class CrmApiKey
    {
		public Guid Id { get; set; }
		public Guid BizdomainId { get; set; } //= RequestData.BizDomainId;
		public Guid PermissionId { get; set; }
		public Guid CreatorId { get; set; }
		public bool IsDisabled { get; set; }
		public DateTime RegisterTime { get; set; } = DateTime.Now;
		public DateTime LastAccessTime { get; set; } = DateTime.Now;
		public bool IsDeleted { get; set; }
	}
}
