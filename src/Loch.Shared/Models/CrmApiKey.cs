using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Models
{
    public class CrmApiKey
    {
        public Guid ApiKeyId { get; set; }

        public Guid BizdomainId { get; set; }

        public Guid PermissionId { get; set; }

        public Guid CreatorId { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime RegisterTime { get; set; }

        public DateTime LastAccessTime { get; set; }

        public bool IsDeleted { get; set; }
        public bool AccBizDomainIsDisabled { get; set; }

        public bool HasFullAccess
        {
            get { return (!AccBizDomainIsDisabled && !IsDisabled) ? (PermissionId == Guid.Empty) : false; }
        }
    }
}