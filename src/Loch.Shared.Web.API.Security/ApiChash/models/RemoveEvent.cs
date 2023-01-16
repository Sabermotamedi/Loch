using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash.models
{
	public abstract class RemoveEvent
	{
		public abstract RemoveEventType Type { get; }
	}
	public enum RemoveEventType
	{
		AbsoluteTime,
		SlidingTime,
		General
	}
}
