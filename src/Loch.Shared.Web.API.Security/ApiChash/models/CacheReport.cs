using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash.models
{
	public struct CacheReport
	{
		public int CacheCount;
		public int IndexedKeyCount, CachedKeyCount;
		public int HitCount, MissCount;
		public int MaximumObjectsCount;

		public CacheReport(
			int CacheCount, int IndexedKeyCount, int CachedKeyCount,
			int HitCount, int MissCount, int MaximumObjectsCount)
		{
			this.CacheCount = CacheCount;
			this.CachedKeyCount = CachedKeyCount;
			this.IndexedKeyCount = IndexedKeyCount;
			this.HitCount = HitCount;
			this.MissCount = MissCount;
			this.MaximumObjectsCount = MaximumObjectsCount;
		}
	}
}
