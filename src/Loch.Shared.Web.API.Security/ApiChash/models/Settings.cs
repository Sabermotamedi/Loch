using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash
{
	internal static class Settings
	{
		public static int MaxObjectsInRam => 10000;// Properties.Settings.Default.MaxObjectsInRam;
		public static double CleanUpRatio => 100000000;// Properties.Settings.Default.CleanUpRatio;
	}
}
