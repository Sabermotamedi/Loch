using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash.models
{
	public class Entry
	{
		public bool IsDeleted;
		public string Key;
		public DateTime LastUpdateTime;
		public DateTime LastVisit;
		public Location Location;
		public CachePriority Priority;
		public int ReferenceNum, VisitNum;

		public Entry()
		{
		}

		internal Entry(string Key, IndexEntry Entry)
		{
			this.Key = Key;
			LastUpdateTime = new DateTime(Entry.LastUpdateTime);
			LastVisit = new DateTime(Entry.LastVisit);
			Location = Entry.Location;
			Priority = Entry.Priority;
			ReferenceNum = Entry.ReferenceNum;
			VisitNum = Entry.VisitNum;
			IsDeleted = Entry.IsDeleted;
		}
	}
}
