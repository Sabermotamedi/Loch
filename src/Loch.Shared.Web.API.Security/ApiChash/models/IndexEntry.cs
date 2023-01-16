using Loch.Shared.Web.API.Security.ApiChash.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash
{
    internal class IndexEntry
    {
		public bool IsDeleted;
		public long LastUpdateTime;
		public long LastVisit;
		public Location Location;
		public CachePriority Priority;
		public int ReferenceNum, VisitNum;
		public RemoveEvent[] RemoveEvents;
	}
}
