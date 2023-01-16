namespace Loch.Shared.Web.API.Security.ApiChash
{
	public struct EntryReport
	{
		public Guid SubsystemId;
		private string Key;
		public DateTime LastUpdateTime;
		public DateTime LastVisit;
		public Location Location;
		public CachePriority Priority;
		public int ReferenceNum, VisitNum;
		public bool IsDeleted;

		internal EntryReport(Guid SubsystemId, string Key, IndexEntry Entry)
		{
			this.SubsystemId = SubsystemId;
			this.Key = Key;
			IsDeleted = Entry.IsDeleted;
			LastUpdateTime = new DateTime(Entry.LastUpdateTime);
			LastVisit = new DateTime(Entry.LastVisit);
			Location = Entry.Location;
			Priority = Entry.Priority;
			VisitNum = Entry.VisitNum;
			ReferenceNum = Entry.ReferenceNum;
		}
	}
}