using Loch.Shared.Web.API.Security.ApiChash.enums;
using Loch.Shared.Web.API.Security.ApiChash.models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash
{
	public static class CacheManager
	{
		private static readonly Guid cacheManagerInstanceId = Guid.NewGuid();
		private static readonly object syncObje = new object();

		private static readonly ConcurrentDictionary<Guid, GenericCache> GlobalCaches =
			new ConcurrentDictionary<Guid, GenericCache>();

		private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, GenericCache>> NamedCaches =
			new ConcurrentDictionary<Guid, ConcurrentDictionary<string, GenericCache>>();

		private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, GenericCache>> UserCaches =
			new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, GenericCache>>();

		private static readonly ConcurrentDictionary<Guid, int> MaxObjectsInRamForUserCaches =
			new ConcurrentDictionary<Guid, int>();

		private static readonly ConcurrentDictionary<Guid, KeysToCleanUpMethod> CleanUpMethodForUserCaches =
			new ConcurrentDictionary<Guid, KeysToCleanUpMethod>();

		public static int DefaultMaximumObjectsInRam => Settings.MaxObjectsInRam;

		public static GenericCache Global
		{
			get;
			//{
			//	var objs = new[]
			//	{
			//		new LocoSubsystemAttribute("00000001-0778-0000-0000-000000000000", "che",
			//			"Armitis.LocoPortal.Framework.Cache")
			//	};
			//	//var objs = new StackTrace().GetFrame(1).GetMethod().Module.Assembly.GetCustomAttributes(typeof(LocoSubsystemAttribute), false);
			//	if (objs.Length == 1)
			//		return GetGlobalCache(objs[0].Id);
			//	throw new LocoSubsystemAttributeNotDefinedException();
			//}
		}

		public static GenericCache CurrentUser
		{
			get;
			//{
			//	var objs = new[]
			//	{
			//		new LocoSubsystemAttribute("00000001-0778-0000-0000-000000000000", "che",
			//			"Armitis.LocoPortal.Framework.Cache")
			//	};
			//	//var objs = new StackTrace().GetFrame(1).GetMethod().Module.Assembly.GetCustomAttributes(typeof(LocoSubsystemAttribute), false);
			//	if (objs.Length == 1)
			//	{
			//		var subsystemId = objs[0].Id;

			//		GenericCache result;
			//		Monitor.Enter(UserCaches);
			//		try
			//		{
			//			if (!UserCaches.ContainsKey(subsystemId))
			//				UserCaches.TryAdd(subsystemId, new ConcurrentDictionary<Guid, GenericCache>());
			//			var UserId = RequestData.UserId;
			//			if (!UserCaches[subsystemId].ContainsKey(UserId))
			//			{
			//				var maxObjectsInRam = Settings.MaxObjectsInRam;

			//				if (MaxObjectsInRamForUserCaches.ContainsKey(subsystemId))
			//					maxObjectsInRam = MaxObjectsInRamForUserCaches[subsystemId];

			//				var cleanUpKeyGetter = CleanUpMethodForUserCaches.ContainsKey(subsystemId)
			//					? CleanUpMethodForUserCaches[subsystemId]
			//					: GenericCache.AccessBasedCleanUpMethod;

			//				UserCaches[subsystemId].TryAdd(UserId,
			//					new GenericCache(cacheManagerInstanceId, GenericCacheType.User, subsystemId, UserId, "",
			//						maxObjectsInRam, GenericCache.RatioBasedCleanUpHelpingMethod, cleanUpKeyGetter));
			//			}

			//			result = UserCaches[subsystemId][UserId];
			//		}
			//		finally
			//		{
			//			Monitor.Exit(UserCaches);
			//		}

			//		return result;
			//	}

			//	throw new LocoSubsystemAttributeNotDefinedException();
			//}
		}

		public static List<EntryReport> GetEntries(
			bool isSubsystemImportant, Guid subsystemGuid,
			bool isLocationImportant, Location location,
			bool isDeletedStateImportant, bool isDeleted,
			bool isKeyImportant, string key)
		{
			var result = new List<EntryReport>();

			Monitor.Enter(GlobalCaches);
			try
			{
				foreach (var Id in GlobalCaches.Keys)
					if (!isSubsystemImportant || Id == subsystemGuid)
					{
						var cache = GlobalCaches[Id];
						Monitor.Enter(cache);
						try
						{
							foreach (var s in cache.Index.Keys)
								if (!isKeyImportant || key == s)
								{
									var I = cache.Index[s];
									if ((!isLocationImportant || location == I.Location) &&
											(!isDeletedStateImportant || isDeleted == I.IsDeleted))
										result.Add(new EntryReport(Id, s, I));
								}
						}
						finally
						{
							Monitor.Exit(cache);
						}
					}
			}
			finally
			{
				Monitor.Exit(GlobalCaches);
			}

			return result;
		}

		public static GenericCache GetGlobalCache(Guid SubsystemId)
		{
			Monitor.Enter(GlobalCaches);
			try
			{
				if (!GlobalCaches.ContainsKey(SubsystemId))
					GlobalCaches.TryAdd(SubsystemId,
						new GenericCache(cacheManagerInstanceId, GenericCacheType.Global, SubsystemId, Guid.Empty, ""));
				return GlobalCaches[SubsystemId];
			}
			finally
			{
				Monitor.Exit(GlobalCaches);
			}
		}

		public static GenericCache GetNamedCache(Guid SubsystemId, string Name)
		{
			return GetNamedCache(SubsystemId, Name, Location.Setting);
		}

		public static GenericCache GetNamedCache(Guid SubsystemId, string Name, Location location)
		{
			Monitor.Enter(NamedCaches);
			try
			{
				if (!NamedCaches.ContainsKey(SubsystemId))
					NamedCaches.TryAdd(SubsystemId, new ConcurrentDictionary<string, GenericCache>());
				if (!NamedCaches[SubsystemId].ContainsKey(Name))
					NamedCaches[SubsystemId].TryAdd(Name,
						new GenericCache(cacheManagerInstanceId, GenericCacheType.Named, SubsystemId, Guid.Empty, Name,
							location: location));

				return NamedCaches[SubsystemId][Name];
			}
			finally
			{
				Monitor.Exit(NamedCaches);
			}
		}

		public static GenericCache GetNamedCache(string Name)
		{
			//var Objs = new[]
			//{
			//	new LocoSubsystemAttribute("00000001-0778-0000-0000-000000000000", "che", "Armitis.LocoPortal.Framework.Cache")
			//};
			////var Objs = new StackTrace().GetFrame(1).GetMethod().Module.Assembly.GetCustomAttributes(typeof(LocoSubsystemAttribute), false);
			//if (Objs.Length == 1)
			//{
			//	var Id = Objs[0].Id;
			//	return GetNamedCache(Id, Name);
			//}

			//throw new LocoSubsystemAttributeNotDefinedException();
			return null;
		}

		public static void SetUserCachesMaximumObjectsInRam(Guid SubsystemId, int MaxObjectsInRam)
		{
			Monitor.Enter(UserCaches);
			try
			{
				if (UserCaches.ContainsKey(SubsystemId))
					foreach (var cache in UserCaches[SubsystemId].Values)
						cache.MaxObjectsInRam = MaxObjectsInRam;
				if (!MaxObjectsInRamForUserCaches.ContainsKey(SubsystemId))
					MaxObjectsInRamForUserCaches.TryAdd(SubsystemId, MaxObjectsInRam);
				else
					MaxObjectsInRamForUserCaches[SubsystemId] = MaxObjectsInRam;
			}
			finally
			{
				Monitor.Exit(UserCaches);
			}
		}

		public static void SetUserCachesCleanUpMethod(Guid SubsystemId, KeysToCleanUpMethod CleanUpMethod)
		{
			Monitor.Enter(UserCaches);
			try
			{
				if (UserCaches.ContainsKey(SubsystemId))
					foreach (var cache in UserCaches[SubsystemId].Values)
						cache.CleanUpKeyGetter = CleanUpMethod;
				if (!CleanUpMethodForUserCaches.ContainsKey(SubsystemId))
					CleanUpMethodForUserCaches.TryAdd(SubsystemId, CleanUpMethod);
				else
					CleanUpMethodForUserCaches[SubsystemId] = CleanUpMethod;
			}
			finally
			{
				Monitor.Exit(UserCaches);
			}
		}

		public static void ResetUserCacheIndices(Guid SubsystemId)
		{
			Monitor.Enter(UserCaches);
			try
			{
				if (!UserCaches.ContainsKey(SubsystemId)) return;

				foreach (var cache in UserCaches[SubsystemId].Values)
					cache.ResetGlobalIndex();
			}
			finally
			{
				Monitor.Exit(UserCaches);
			}
		}

		public static void ResetUserCaches(Guid SubsystemId)
		{
			Monitor.Enter(UserCaches);
			try
			{
				if (!UserCaches.ContainsKey(SubsystemId)) return;

				foreach (var cache in UserCaches[SubsystemId].Values)
					cache.ResetGlobalCache();
			}
			finally
			{
				Monitor.Exit(UserCaches);
			}
		}

		public static CacheReport GenerateUserCachesReport(Guid SubsystemId)
		{
			Monitor.Enter(UserCaches);
			try
			{
				var Report = new CacheReport(0, 0, 0, 0, 0, 0);
				if (!UserCaches.ContainsKey(SubsystemId))
					return Report;
				else if (MaxObjectsInRamForUserCaches.ContainsKey(SubsystemId))
					Report.MaximumObjectsCount = MaxObjectsInRamForUserCaches[SubsystemId];
				else
					Report.MaximumObjectsCount = Settings.MaxObjectsInRam;

				foreach (var cache in UserCaches[SubsystemId].Values)
				{
					var LocalReport = cache.GenerateReport();

					Report.CacheCount++;
					Report.CachedKeyCount += LocalReport.CachedKeyCount;
					Report.IndexedKeyCount += LocalReport.IndexedKeyCount;
					Report.HitCount += LocalReport.HitCount;
					Report.MissCount += LocalReport.MissCount;
				}

				return Report;
			}
			finally
			{
				Monitor.Exit(UserCaches);
			}
		}

		public static List<string> GetNamedCacheList(Guid SubsystemId)
		{
			Monitor.Enter(NamedCaches);
			try
			{
				var NamedCacheList = new List<string>();

				if (NamedCaches.ContainsKey(SubsystemId))
					foreach (var Name in NamedCaches[SubsystemId].Keys)
						NamedCacheList.Add(Name);

				return NamedCacheList;
			}
			finally
			{
				Monitor.Exit(NamedCaches);
			}
		}

		public static void ResetAll()
		{
			Monitor.Enter(UserCaches);
			try
			{
				foreach (var subsystemId in UserCaches.Keys)
				{
					foreach (var cache in UserCaches[subsystemId].Values)
					{
						cache.ResetGlobalIndex();
						cache.ResetGlobalCache();
					}

					foreach (var cache in NamedCaches[subsystemId].Values)
					{
						cache.ResetGlobalIndex();
						cache.ResetGlobalCache();
					}

					GlobalCaches[subsystemId].ResetGlobalIndex();
					GlobalCaches[subsystemId].ResetGlobalCache();
				}
			}
			finally
			{
				Monitor.Exit(UserCaches);
			}
		}

		public static class Distributed
		{
			internal static void Log(string role, Guid instanceId, Guid subsystemId,
				GenericCacheType cacheType, Guid userId, string name, List<string> cacheKeys)
			{
				Log($"{role},\t{instanceId},\t{subsystemId},\t{cacheType},\t{userId},\t{name},\t{string.Join(",", cacheKeys)}");
			}

			internal static void Log(string str)
			{
				if (true)//!Properties.Settings.Default.EnableLogging)
					return;
				lock (syncObje)
				{
					try
					{
						var logFile = File.AppendText("");//Properties.Settings.Default.LogFile);
						try
						{
							logFile.WriteLine($"{DateTime.Now}\t{str}");
						}
						finally
						{
							logFile.Close();
						}
					}
					catch
					{
						// ignored
					}
				}
			}

			public static class Client
			{
				//private static readonly ConcurrentDictionary<string, CacheControlService> services =
				//	new ConcurrentDictionary<string, CacheControlService>();

				//private static bool Proceed()
				//{
				//	return Properties.Settings.Default.DistributedMode;
				//}

				//private static CacheControlService GetService(string serviceAddress)
				//{
				//	if (string.IsNullOrEmpty(serviceAddress)) return null;

				//	if (!Monitor.TryEnter(services, 1000)) throw new Exception("Cannot obtain lock on `services.");

				//	try
				//	{
				//		CacheControlService result = null;
				//		if (services.ContainsKey(serviceAddress))
				//			result = services[serviceAddress];
				//		if (result == null)
				//		{
				//			result = new CacheControlService { Url = serviceAddress };
				//			services.TryAdd(serviceAddress, result);
				//		}

				//		return result;
				//	}
				//	finally
				//	{
				//		Monitor.Exit(services);
				//	}
				//}

				//private static void AsyncResetCacheObjects(RemoveCacheEntryState rces)
				//{
				//	try
				//	{
				//		Log("Commander:\tResetCacheObjects", rces.InstanceId, rces.SubsystemId, rces.CacheType, rces.UserId,
				//			rces.Name, new List<string> { "" });
				//		foreach (var serviceAddress in Properties.Settings.Default.DistributedServices)
				//			try
				//			{
				//				var service = GetService(serviceAddress);
				//				service?.ResetCacheObjects(rces.InstanceId, rces.SubsystemId, (int)rces.CacheType, rces.UserId,
				//					rces.Name);
				//			}
				//			catch (Exception ex)
				//			{
				//				Log($"{ex.Message}: {ex.StackTrace}");
				//			}
				//	}
				//	finally
				//	{
				//		rces.Timer?.Dispose();
				//	}
				//}

				//internal static void ResetCacheObjects(Guid instanceId, Guid subsystemId, GenericCacheType cacheType,
				//	Guid userId, string name)
				//{
				//	if (!Proceed())
				//		return;

				//	var state = new RemoveCacheEntryState(instanceId, subsystemId, cacheType, userId, name,
				//		new List<string> { "" });
				//	var timer = new Timer(o => AsyncResetCacheObjects(state),
				//		null,
				//		Properties.Settings.Default.DistributedModeDelaySecons * 1000,
				//		Timeout.Infinite);
				//	state.Timer = timer;
				//}

				//private static void AsyncResetCacheIndex(object state)
				//{
				//	var rces = state as RemoveCacheEntryState;
				//	try
				//	{
				//		Log("Commander:\tResetCacheIndex", rces.InstanceId, rces.SubsystemId, rces.CacheType, rces.UserId, rces.Name, "");
				//		foreach (var serviceAddress in Properties.Settings.Default.DistributedServices)
				//		{
				//			try
				//			{
				//				var service = GetService(serviceAddress);
				//				CommonUtility.RunAction(() =>
				//				{
				//					try { service?.ResetCacheIndex(rces.InstanceId, rces.SubsystemId, (int)rces.CacheType, rces.UserId, rces.Name); }
				//					catch (Exception e) { Log(e.Message + ": " + e.StackTrace); }
				//				});
				//			}
				//			catch (Exception ex)
				//			{
				//				Log(ex.Message + ": " + ex.StackTrace);
				//			}
				//		}
				//	}
				//	finally
				//	{
				//		if (rces.Timer != null)
				//			rces.Timer.Dispose();
				//	}
				//}

				//internal static void ResetCacheIndex(Guid instanceId, Guid subsystemId, GenericCacheType cacheType, Guid userId,
				//	string name)
				//{
				//	if (!Proceed())
				//		return;

				//	var state = new RemoveCacheEntryState(instanceId, subsystemId, cacheType, userId, name,
				//		new List<string> { "" });
				//	var timer = new Timer(AsyncRemoveCacheEntry,
				//		state,
				//		Properties.Settings.Default.DistributedModeDelaySecons * 1000,
				//		Timeout.Infinite);

				//	state.Timer = timer;
				//}

				//private static void AsyncRemoveCacheEntry(object state)
				//{
				//	var rces = (RemoveCacheEntryState)state;
				//	try
				//	{
				//		CommonUtility.RunAction(() =>
				//		{
				//			Log("Commander:\tRemoveCacheEntry", rces.InstanceId, rces.SubsystemId, rces.CacheType, rces.UserId,
				//				rces.Name, rces.CacheKeys);
				//			foreach (var serviceAddress in Properties.Settings.Default.DistributedServices)
				//				try
				//				{
				//					var service = GetService(serviceAddress);
				//					try
				//					{
				//						service?.RemoveEntry(rces.InstanceId, rces.SubsystemId, (int)rces.CacheType, rces.UserId,
				//							rces.Name, rces.CacheKeys.ToArray());
				//					}
				//					catch (Exception e)
				//					{
				//						Log(e.Message + ": " + e.StackTrace);
				//					}
				//				}
				//				catch (Exception ex)
				//				{
				//					Log(ex.Message + ": " + ex.StackTrace);
				//				}
				//		});
				//	}
				//	catch (Exception ex)
				//	{
				//		Log(ex.Message + ": " + ex.StackTrace);
				//	}
				//	finally
				//	{
				//		rces?.Timer?.Dispose();
				//	}
				//}

				//internal static void RemoveCacheEntry(Guid instanceId, Guid subsystemId, GenericCacheType cacheType,
				//	Guid userId, string name, params string[] cacheKeys)
				//{
				//	if (!Proceed())
				//		return;

				//	try
				//	{
				//		var state = new RemoveCacheEntryState(instanceId, subsystemId, cacheType, userId, name,
				//			new List<string>(cacheKeys));
				//		var timer = new Timer(AsyncRemoveCacheEntry,
				//			state,
				//			Properties.Settings.Default.DistributedModeDelaySecons * 1000,
				//			Timeout.Infinite);

				//		state.Timer = timer;
				//	}
				//	catch (Exception ex)
				//	{
				//		Log(ex.Message + ". " + ex.StackTrace);
				//	}
				//}

				//public static void SetDataUpdates(Guid bizdomainId, ConcurrentDictionary<string, DateTime> data)
				//{
				//	if (!Proceed())
				//		return;

				//	if (Properties.Settings.Default.DistributedModeDelaySecons == 0)
				//	{
				//		CommonUtility.RunAction(() => SetDataUpdatesOnOtherServers(bizdomainId, data));
				//	}
				//	else
				//	{
				//		var timer = new System.Timers.Timer
				//		{ Interval = Properties.Settings.Default.DistributedModeDelaySecons * 1000 };
				//		timer.Elapsed += (sender, args) =>
				//		{
				//			try
				//			{
				//				SetDataUpdatesOnOtherServers(bizdomainId, data);
				//			}
				//			finally
				//			{
				//				((System.Timers.Timer)sender).Dispose();
				//			}
				//		};
				//		timer.Start();
				//	}
				//}

				//private static void SetDataUpdatesOnOtherServers(Guid bizdomainId, ConcurrentDictionary<string, DateTime> data)
				//{
				//	foreach (var serviceAddress in Properties.Settings.Default.DistributedServices)
				//		try
				//		{
				//			var service = GetService(serviceAddress);
				//			service?.SetDataUpdates(bizdomainId, CommonConvertor.Json.Serialize(data));
				//		}
				//		catch (Exception ex)
				//		{
				//			Log($"{ex.Message}: {ex.StackTrace}");
				//		}
				//}

				//private class RemoveCacheEntryState
				//{
				//	public RemoveCacheEntryState(Guid instanceId, Guid subsystemId, GenericCacheType cacheType, Guid userId,
				//		string name, List<string> cacheKeys)
				//	{
				//		InstanceId = instanceId;
				//		SubsystemId = subsystemId;
				//		CacheType = cacheType;
				//		UserId = userId;
				//		Name = name;
				//		CacheKeys = cacheKeys;
				//	}

				//	public Guid InstanceId { get; }
				//	public Guid SubsystemId { get; }
				//	public GenericCacheType CacheType { get; }
				//	public Guid UserId { get; }
				//	public string Name { get; }
				//	public List<string> CacheKeys { get; }
				//	public Timer Timer { get; set; }
				//}
			}

			public static class Server
			{
				private static bool Proceed(Guid instanceId)
				{
					if (true)//!Properties.Settings.Default.DistributedMode)
						return false;

					//check for instanceId
					if (instanceId == cacheManagerInstanceId)
						return false;

					return true;
				}

				private static GenericCache GetCache(ref Guid subsystemId, GenericCacheType type, ref Guid userId,
					string cacheName)
				{
					GenericCache cache = null;
					switch (type)
					{
						case GenericCacheType.Global:
							if (GlobalCaches.ContainsKey(subsystemId))
								cache = GlobalCaches[subsystemId];
							break;
						case GenericCacheType.User:
							if (UserCaches.ContainsKey(subsystemId))
								if (UserCaches[subsystemId].ContainsKey(userId))
									cache = UserCaches[subsystemId][userId];
							break;
						case GenericCacheType.Named:
							if (NamedCaches.ContainsKey(subsystemId))
								if (NamedCaches[subsystemId].ContainsKey(cacheName))
									cache = NamedCaches[subsystemId][cacheName];
							break;
					}

					return cache;
				}

				public static void ResetAllCacheObjects(Guid instanceId)
				{
					try
					{
						if (!Proceed(instanceId))
							return;

						Log("Listener:\tResetAllCacheObjects", instanceId, Guid.Empty, GenericCacheType.Global, Guid.Empty, "",
							new List<string> { "" });
						foreach (var cache in GlobalCaches.Values)
							cache.ResetLocalCache();
						foreach (var subsystemNamedCaches in NamedCaches.Values)
							foreach (var namedCache in subsystemNamedCaches.Values)
								namedCache.ResetLocalCache();

						foreach (var subsystemUserCaches in UserCaches.Values)
							foreach (var userCache in subsystemUserCaches.Values)
								userCache.ResetLocalCache();
					}
					catch (Exception ex)
					{
						Log(ex.Message + ": " + ex.StackTrace);
					}
				}

				public static void ResetAllCacheIndexes(Guid instanceId)
				{
					try
					{
						if (!Proceed(instanceId))
							return;

						Log("Listener:\tResetAllCacheIndexes", instanceId, Guid.Empty, GenericCacheType.Global, Guid.Empty, "",
							new List<string> { "" });
						foreach (var cache in GlobalCaches.Values)
							cache.ResetLocalIndex();
						foreach (var subsystemNamedCaches in NamedCaches.Values)
							foreach (var namedCache in subsystemNamedCaches.Values)
								namedCache.ResetLocalIndex();

						foreach (var subsystemUserCaches in UserCaches.Values)
							foreach (var userCache in subsystemUserCaches.Values)
								userCache.ResetLocalIndex();
					}
					catch (Exception ex)
					{
						Log(ex.Message + ": " + ex.StackTrace);
					}
				}

				public static void ResetCacheObjects(Guid instanceId, Guid subsystemId, GenericCacheType type, Guid userId,
					string cacheName)
				{
					try
					{
						if (!Proceed(instanceId))
							return;

						Log("Listener:\tResetCacheObjects", instanceId, subsystemId, type, userId, cacheName,
							new List<string> { "" });
						var cache = GetCache(ref subsystemId, type, ref userId, cacheName);

						cache?.ResetLocalCache();
					}
					catch (Exception ex)
					{
						Log(ex.Message + ": " + ex.StackTrace);
					}
				}

				public static void ResetCacheIndex(Guid instanceId, Guid subsystemId, GenericCacheType type, Guid userId,
					string cacheName)
				{
					try
					{
						if (!Proceed(instanceId))
							return;

						Log("Listener:\tResetCacheIndex", instanceId, subsystemId, type, userId, cacheName, new List<string> { "" });
						var cache = GetCache(ref subsystemId, type, ref userId, cacheName);

						cache?.ResetLocalIndex();
					}
					catch (Exception ex)
					{
						Log(ex.Message + ": " + ex.StackTrace);
					}
				}

				public static void RemoveEntry(Guid instanceId, Guid subsystemId, GenericCacheType type, Guid userId,
					string cacheName, List<string> cacheKeys)
				{
					try
					{
						if (!Proceed(instanceId))
							return;

						Log("Listener:\tRemoveEntry", instanceId, subsystemId, type, userId, cacheName, cacheKeys);
						var cache = GetCache(ref subsystemId, type, ref userId, cacheName);

						if (cache != null)
							cacheKeys.ForEach(key => cache.RemoveLocal(key));
						else
							Log("Listenr:\tError, Cache Is Null", instanceId, subsystemId, type, userId, cacheName, cacheKeys);
					}
					catch (Exception ex)
					{
						Log(ex.Message + ": " + ex.StackTrace);
					}
				}
			}
		}
	}
}
