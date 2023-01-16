using Loch.Shared.Web.API.Security.ApiChash.enums;
using Loch.Shared.Web.API.Security.ApiChash.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.ApiChash
{
	public delegate int NumberOfEntriesToClean(GenericCache Cache);

	public delegate IEnumerable<string> KeysToCleanUpMethod(GenericCache Cache, int NumberOfItemsToRemove);

	public sealed class GenericCache
	{
		private static readonly HttpClient elastic;
		private static readonly ConnectionMultiplexer redis;

		private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
		{ ContractResolver = new WritablePropertiesOnlyResolver() };

		static GenericCache()
		{
			try
			{
				var url = new Uri("");//ConfigurationManager.ConnectionStrings["Elastic"].ConnectionString);
				elastic = new HttpClient
				{
					BaseAddress = url,
					Timeout = TimeSpan.FromSeconds(1)
				};
				if (!string.IsNullOrEmpty(url.UserInfo))
					elastic.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
						Convert.ToBase64String(Encoding.ASCII.GetBytes(url.UserInfo)));

				redis = ConnectionMultiplexer.Connect("redisConnection"); //ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
			}
			catch (Exception e) { }
		}

		public GenericCache(Guid instanceId, GenericCacheType cacheType, Guid subsystemId, Guid userId, string name)
		{
			initialize(instanceId, cacheType, subsystemId, userId, name);
		}

		public GenericCache(Guid instanceId, GenericCacheType cacheType, Guid subsystemId, Guid userId, string name,
			int MaxObjectsInRam)
		{
			initialize(instanceId, cacheType, subsystemId, userId, name, MaxObjectsInRam);
		}

		public GenericCache(Guid instanceId, GenericCacheType cacheType, Guid subsystemId, Guid userId, string name,
			int MaxObjectsInRam, double CleanUpRatio)
		{
			if (CleanUpRatio < 0) CleanUpRatio = 0;
			else if (CleanUpRatio > 1) CleanUpRatio = 1;
			initialize(instanceId, cacheType, subsystemId, userId, name, MaxObjectsInRam, cleanUpRatio: CleanUpRatio);
		}

		public GenericCache(Guid instanceId, GenericCacheType cacheType, Guid subsystemId, Guid userId, string name,
			int MaxObjectsInRam, NumberOfEntriesToClean CleanUpHelpingMethod)
		{
			initialize(instanceId, cacheType, subsystemId, userId, name, MaxObjectsInRam, CleanUpHelpingMethod);
		}

		public GenericCache(Guid instanceId, GenericCacheType cacheType, Guid subsystemId, Guid userId, string name,
			int MaxObjectsInRam, NumberOfEntriesToClean CleanUpHelpingMethod, KeysToCleanUpMethod CleanUpKeyGetter)
		{
			initialize(instanceId, cacheType, subsystemId, userId, name, MaxObjectsInRam, CleanUpHelpingMethod,
				CleanUpKeyGetter);
		}

		public GenericCache(Guid instanceId, GenericCacheType cacheType, Guid subsystemId, Guid userId, string name,
			int maxObjectsInRam = -1,
			NumberOfEntriesToClean cleanUpHelpingMethod = null, KeysToCleanUpMethod cleanUpKeyGetter = null,
			double cleanUpRatio = -1, Location location = Location.Ram)
		{
			initialize(instanceId, cacheType, subsystemId, userId, name, maxObjectsInRam, cleanUpHelpingMethod,
				cleanUpKeyGetter, cleanUpRatio, location);
		}

		public EventHandler<string> EntryRemovedEvent { get; set; }
		public EventHandler<string> CacheResetEvent { get; set; }

		private Guid InstanceId { get; set; } = Guid.Empty;
		private Guid SubsystemId { get; set; } = Guid.Empty;
		private Guid UserId { get; set; } = Guid.Empty;
		private string Name { get; set; } = "";
		private GenericCacheType CacheType { get; set; } = GenericCacheType.Global;

		private Location Location { get; set; } = Location.Ram;

		private ConcurrentDictionary<string, object> RamEntries { get; } = new ConcurrentDictionary<string, object>();

		internal ConcurrentDictionary<string, IndexEntry> Index { get; set; } =
			new ConcurrentDictionary<string, IndexEntry>();

		private int maxObjectsInRam { get; set; }
		private double cleanUpRatio { get; set; }
		private int[] CountsByPrioirities { get; } = new int[5] { 0, 0, 0, 0, 0 };
		private NumberOfEntriesToClean CleanUpHelpingMethod { get; set; }
		public KeysToCleanUpMethod CleanUpKeyGetter { get; set; }

		public int Count =>
			GetCountByPriority(CachePriority.High) +
			GetCountByPriority(CachePriority.Low) +
			GetCountByPriority(CachePriority.Normal) +
			GetCountByPriority(CachePriority.VeryHigh) +
			GetCountByPriority(CachePriority.VeryLow);

		internal int MaxObjectsInRam
		{
			get { return maxObjectsInRam; }
			set
			{
				Monitor.Enter(this);
				try
				{
					maxObjectsInRam = value;
					if (Count < maxObjectsInRam)
						CleanUp();
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}

		public object this[string Key] => GetCachedEntry(Key);

		private void GenericCache_RemoveEvent(string Key)
		{
			RemoveGlobal(Key);
		}

		private static int CompareByAccess(Entry x, Entry y)
		{
			if ((int)x.Priority > (int)y.Priority)
				return 1;
			if ((int)x.Priority < (int)y.Priority)
				return -1;
			return x.ReferenceNum - y.ReferenceNum;
		}

		private static int CompareByFIFO(Entry x, Entry y)
		{
			if ((int)x.Priority > (int)y.Priority)
				return 1;
			if ((int)x.Priority < (int)y.Priority)
				return -1;
			if (x.LastVisit < y.LastVisit)
				return -1;
			return 1;
		}

		private long GetExpiration(IndexEntry Ix)
		{
			//long Result = DateTime.MaxValue.Ticks, Temp;
			//for (var i = 0; i < Ix.RemoveEvents.Length; i++)
			//	switch (Ix.RemoveEvents[i].Type)
			//	{
			//		case RemoveEventType.AbsoluteTime:
			//			if ((Ix.RemoveEvents[i] as AbsoluteTimeRemoveEvent).ExpirationTime.Ticks < Result)
			//				Result = (Ix.RemoveEvents[i] as AbsoluteTimeRemoveEvent).ExpirationTime.Ticks;
			//			break;
			//		case RemoveEventType.SlidingTime:
			//			Temp = Ix.LastVisit;
			//			Temp += (Ix.RemoveEvents[i] as SlidingTimeRemoveEvent).DeltaTime.Ticks;
			//			if (Temp < Result)
			//				Result = Temp;
			//			break;
			//	}

			return 1;// Result;
		}

		private void _Remove(params string[] keys)
		{
			keys.ToList().ForEach(key => EntryRemovedEvent?.Invoke(null, key));

			Monitor.Enter(this);
			try
			{
				var list = keys.Where(key => Index.ContainsKey(key) && !Index[key].IsDeleted).ToList();
				list.ForEach(key =>
				{
					var I = Index[key];
					I.IsDeleted = true;
					Index[key] = I;
					CountsByPrioirities[(int)I.Priority]--;
				});

				switch (Location)
				{
					case Location.Ram:
						object removed;
						foreach (var key in list)
							RamEntries.TryRemove(key, out removed);
						break;
					case Location.Elasticsearch:
						throw new Exception("Elasticsearch caching doesn't support sync method");
					case Location.Redis:
						rdRemove(list.ToArray());
						break;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		private async Task _RemoveAsync(params string[] keys)
		{
			keys.ToList().ForEach(key => EntryRemovedEvent?.Invoke(null, key));

			Monitor.Enter(this);
			try
			{
				var list = keys.Where(key => Index.ContainsKey(key) && !Index[key].IsDeleted).ToList();
				list.ForEach(key =>
				{
					var I = Index[key];
					I.IsDeleted = true;
					Index[key] = I;
					CountsByPrioirities[(int)I.Priority]--;
				});

				switch (Location)
				{
					case Location.Ram:
						object removed;
						foreach (var key in list)
							RamEntries.TryRemove(key, out removed);
						break;
					case Location.Elasticsearch:
						await esRemove(list.ToArray());
						break;
					case Location.Redis:
						rdRemove(list.ToArray());
						break;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		private List<string> _Remove<T>(Guid? bizDomainId = null, Expression<Func<T, object>> bizdomainProperty = null)
		{
			//var removes = new List<string>();
			//switch (Location)
			//{
			//	case Location.Ram:
			//		var properyInfo = CommonUtility.Reflection.GetPropertyInfo(bizdomainProperty);
			//		removes = GetKeys();
			//		if (bizDomainId.HasValue)
			//			try { removes = removes.Where(_ => (Guid)properyInfo.GetValue(Get<T>(_)) == bizDomainId).ToList(); }
			//			catch { }
			//		_Remove(removes.ToArray());
			//		break;
			//	case Location.Elasticsearch:
			//		throw new Exception("Elasticsearch caching doesn't support sync method");
			//	case Location.Redis:
			//		removes = rdGetKeys(bizDomainId);
			//		_Remove(removes.ToArray());
			//		break;
			//}

			return new List<string>();// removes;
		}

		private async Task<List<string>> _RemoveAsync<T>(Guid? bizDomainId = null,
			Expression<Func<T, object>> bizdomainProperty = null)
		{
			var removes = new List<string>();
			switch (Location)
			{
				case Location.Ram:
					return _Remove(bizDomainId, bizdomainProperty);
				case Location.Elasticsearch:
					removes = await esGetKeys(bizDomainId);
					await _RemoveAsync(removes.ToArray());
					break;
				case Location.Redis:
					removes = rdGetKeys(bizDomainId);
					_Remove(removes.ToArray());
					break;
			}

			return removes;
		}

		private bool _Contains(string Key, bool removeIfExpired = true)
		{
			if (Index.ContainsKey(Key))
			{
				var I = Index[Key];
				if (I.IsDeleted) return false;

				if (GetExpiration(I) > DateTime.Now.Ticks) return true;

				if (removeIfExpired) _Remove(Key);
				return false;
			}

			return false;
		}

		private async Task<bool> _ContainsAsync(string Key, bool removeIfExpired = true)
		{
			if (Index.ContainsKey(Key))
			{
				var I = Index[Key];
				if (I.IsDeleted) return false;

				if (GetExpiration(I) > DateTime.Now.Ticks) return true;

				if (removeIfExpired) await _RemoveAsync(Key);
				return false;
			}

			return false;
		}

		private void CleanUp()
		{
			Monitor.Enter(this);
			try
			{
				var n = CleanUpHelpingMethod(this);
				if (n <= 0)
				{
					n = 0;
					if (Count - n >= maxObjectsInRam)
						n = Count - maxObjectsInRam + 1;
				}

				if (n > Count)
					n = Count;

				if (Count > 0)
					_Remove(CleanUpKeyGetter(this, n).ToArray());
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		private async Task CleanUpAsync()
		{
			Monitor.Enter(this);
			try
			{
				var n = CleanUpHelpingMethod(this);
				if (n <= 0)
				{
					n = 0;
					if (Count - n >= maxObjectsInRam)
						n = Count - maxObjectsInRam + 1;
				}

				if (n > Count)
					n = Count;

				if (Count > 0)
					await _RemoveAsync(CleanUpKeyGetter(this, n).ToArray());
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public static int RatioBasedCleanUpHelpingMethod(GenericCache Cache)
		{
			return Cache.Count - (int)(Cache.Count * Cache.cleanUpRatio);
		}

		public static IEnumerable<string> FIFOBasedCleanUpMethod(GenericCache Cache, int NumberOfItemsToRemove)
		{
			var Entries = Cache.GetCachedEntries();
			Entries.Sort(CompareByFIFO);

			var KeysToRemove = new string[NumberOfItemsToRemove];
			for (var i = 0; i < NumberOfItemsToRemove; i++)
				KeysToRemove[i] = Entries[i].Key;

			return KeysToRemove;
		}

		public static IEnumerable<string> AccessBasedCleanUpMethod(GenericCache Cache, int NumberOfItemsToRemove)
		{
			var Entries = Cache.GetCachedEntries();
			Entries.Sort(CompareByFIFO);

			var KeysToRemove = new string[NumberOfItemsToRemove];
			for (var i = 0; i < NumberOfItemsToRemove; i++)
				KeysToRemove[i] = Entries[i].Key;

			return KeysToRemove;
		}

		public bool Contains(string Key)
		{
			return _Contains(Key);
		}

		public async Task<bool> ContainsAsync(string Key)
		{
			return await _ContainsAsync(Key);
		}

		public List<T> Get<T, TKey>(List<TKey> keys, Func<List<TKey>, Dictionary<TKey, T>> load)
		{
			keys = keys.Distinct().ToList();
			var hits = keys.Where(id => Contains(id.ToString()))
				.Select(id => new KeyValuePair<string, T>(id.ToString(), Get<T>(id.ToString())))
				.Where(item => item.Value != null).ToDictionary(_ => _.Key, _ => _.Value);
			var misseds = load(keys.Where(id => !hits.Keys.Contains(id.ToString())).ToList());
			if (misseds?.Any() == true)
				Task.Run(() => Parallel.ForEach(misseds, item => Add(item.Key.ToString(), item.Value)));
			return keys.Select(id =>
					misseds.ContainsKey(id) ? misseds[id] :
					hits.ContainsKey(id.ToString()) ? hits[id.ToString()] : Get<T>(id.ToString()))
				.Where(_ => _ != null).ToList();
		}

		public async Task<List<T>> GetAsync<T, TKey>(List<TKey> keys, Func<List<TKey>, Dictionary<TKey, T>> load)
		{
			var missedIds = new List<TKey>();
			foreach (var key in keys)
				if (!await ContainsAsync(key.ToString()))
					missedIds.Add(key);
			var misseds = load(missedIds);
			if (misseds?.Count > 0)
				Task.WaitAll(misseds.Select(item => AddAsync(item.Key.ToString(), item.Value)).ToArray());
			return keys.Select(id => misseds.ContainsKey(id) ? misseds[id] : Get<T>(id.ToString()))
				.Where(item => item != null).ToList();
		}

		public T Get<T>(string key)
		{
			var item = GetCachedEntry(key);
			switch (Location)
			{
				case Location.Ram:
					return (T)item;
				case Location.Elasticsearch:
					if (item is JObject) return (item as JObject).ToObject<T>();
					break;
				case Location.Redis:
					if (item is JObject) return (item as JObject).ToObject<T>();
					break;
			}

			return default;
		}

		public async Task<T> GetAsync<T>(string key)
		{
			return await GetCachedEntryAsync<T>(key);
		}

		public object GetCachedEntry(string Key)
		{
			return GetCachedEntry<object>(Key);
		}

		public T GetCachedEntry<T>(string Key)
		{
			T Result;

			if (!Index.ContainsKey(Key))
			{
				Monitor.Enter(this);
				try
				{
					//recheck after acquiring the lock
					if (!Index.ContainsKey(Key))
					{
						var Index = new IndexEntry();
						Index.IsDeleted = true;
						Index.LastUpdateTime = DateTime.UtcNow.Ticks;
						Index.LastVisit = DateTime.UtcNow.Ticks;
						Index.Location = Location;
						Index.Priority = CachePriority.Normal;
						Index.ReferenceNum = 0;
						Index.RemoveEvents = new RemoveEvent[0];
						Index.VisitNum = 0;

						this.Index.AddOrUpdate(Key, Index, (key, value) => Index);
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}

			var I = Index[Key];

			I.ReferenceNum++;

			Result = default;
			if (_Contains(Key))
			{
				var exists = false;
				switch (Location)
				{
					case Location.Ram:
						exists = RamEntries.ContainsKey(Key);
						if (exists) Result = (T)RamEntries[Key];
						break;
					case Location.Elasticsearch:
						throw new Exception("Elasticsearch caching doesn't support sync method");
					case Location.Redis:
						try
						{
							exists = rdExists(Key);
							if (exists) Result = rdGet<T>(Key);
						}
						catch
						{
							exists = false;
							Location = Location.Ram;
						}
						break;
				}

				if (exists)
				{
					I.VisitNum++;
					I.LastVisit = DateTime.UtcNow.Ticks;
				}
			}

			//Index[Key] = I;

			return Result;
		}

		public async Task<T> GetCachedEntryAsync<T>(string Key)
		{
			T Result;

			if (!Index.ContainsKey(Key))
			{
				Monitor.Enter(this);
				try
				{
					//recheck after acquiring the lock
					if (!Index.ContainsKey(Key))
					{
						var Index = new IndexEntry();
						Index.IsDeleted = true;
						Index.LastUpdateTime = DateTime.UtcNow.Ticks;
						Index.LastVisit = DateTime.UtcNow.Ticks;
						Index.Location = Location;
						Index.Priority = CachePriority.Normal;
						Index.ReferenceNum = 0;
						Index.RemoveEvents = new RemoveEvent[0];
						Index.VisitNum = 0;

						this.Index.AddOrUpdate(Key, Index, (key, value) => Index);
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}

			var I = Index[Key];

			I.ReferenceNum++;

			Result = default;
			if (_Contains(Key))
			{
				var exists = false;
				switch (Location)
				{
					case Location.Ram:
						exists = RamEntries.ContainsKey(Key);
						if (exists) Result = (T)RamEntries[Key];
						break;
					case Location.Elasticsearch:
						exists = await esExists(Key);
						if (exists) Result = await esGet<T>(Key);
						break;
					case Location.Redis:
						exists = rdExists(Key);
						if (exists) Result = rdGet<T>(Key);
						break;
				}

				if (exists)
				{
					I.VisitNum++;
					I.LastVisit = DateTime.UtcNow.Ticks;
				}
			}

			return Result;
		}

		private void AddRemoveEvents(ref IndexEntry I, RemoveEvent[] RemoveEvents)
		{
			var Count = 0;
			for (var i = 0; i < RemoveEvents.Length; i++)
				if (RemoveEvents[i].Type == RemoveEventType.AbsoluteTime ||
						RemoveEvents[i].Type == RemoveEventType.SlidingTime)
					Count++;

			I.RemoveEvents = new RemoveEvent[Count];
			for (int i = 0, j = 0; i < RemoveEvents.Length; i++)
				switch (RemoveEvents[i].Type)
				{
					case RemoveEventType.AbsoluteTime:
					case RemoveEventType.SlidingTime:
						I.RemoveEvents[j] = RemoveEvents[i];
						j++;
						break;
					//case RemoveEventType.General:
						//(RemoveEvents[i] as GeneralRemoveEvent).RemoveEvent += GenericCache_RemoveEvent;
						break;
				}
		}

		public void Add(string Key, object Item)
		{
			Add(Key, Item, CachePriority.Normal);
		}

		public void Add(string Key, object Item, CachePriority Priority, params RemoveEvent[] RemoveEvents)
		{
			if (RemoveEvents == null)
				RemoveEvents = new RemoveEvent[0];
			IndexEntry I;
			Monitor.Enter(this);

			try
			{
				if (Count >= MaxObjectsInRam)
					CleanUp();

				if (Index.ContainsKey(Key
				))
				{
					I = Index[Key];
					if (I.IsDeleted)
					{
						CountsByPrioirities[(int)Priority]++;
						I.IsDeleted = false;
						switch (Location)
						{
							case Location.Ram:
								RamEntries.AddOrUpdate(Key, Item, (key, value) => Item);
								break;
							case Location.Elasticsearch:
								throw new Exception("Elasticsearch caching doesn't support sync method");
							case Location.Redis:
								rdAdd(Key, Item);
								break;
						}
					}
					else
					{
						CountsByPrioirities[(int)I.Priority]--;
						CountsByPrioirities[(int)Priority]++;
						switch (Location)
						{
							case Location.Ram:
								RamEntries[Key] = Item;
								break;
							case Location.Elasticsearch:
								throw new Exception("Elasticsearch caching doesn't support sync method");
							case Location.Redis:
								rdAdd(Key, Item);
								break;
						}
					}

					I.LastUpdateTime = DateTime.UtcNow.Ticks;
					I.Location = Location;
					I.Priority = Priority;
					AddRemoveEvents(ref I, RemoveEvents);

					Index[Key] = I;
				}
				else
				{
					CountsByPrioirities[(int)Priority]++;

					I = new IndexEntry();
					I.LastUpdateTime = DateTime.UtcNow.Ticks;
					I.LastVisit = DateTime.UtcNow.Ticks;
					I.Location = Location;
					I.Priority = Priority;
					I.VisitNum = 0;
					I.ReferenceNum = 0;
					AddRemoveEvents(ref I, RemoveEvents);

					switch (Location)
					{
						case Location.Ram:
							RamEntries.AddOrUpdate(Key, Item, (key, value) => Item);
							break;
						case Location.Elasticsearch:
							throw new Exception("Elasticsearch caching doesn't support sync method");
						case Location.Redis:
							rdAdd(Key, Item);
							break;
					}

					Index.AddOrUpdate(Key, I, (key, value) => I);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public async Task AddAsync<T>(string Key, T Item)
		{
			await AddAsync(Key, Item, CachePriority.Normal);
		}

		public async Task AddAsync<T>(string Key, T Item, CachePriority Priority, params RemoveEvent[] RemoveEvents)
		{
			if (RemoveEvents == null)
				RemoveEvents = new RemoveEvent[0];
			IndexEntry I;
			Monitor.Enter(this);

			try
			{
				if (Count >= MaxObjectsInRam)
					await CleanUpAsync();

				if (Index.ContainsKey(Key
				))
				{
					I = Index[Key];
					if (I.IsDeleted)
					{
						CountsByPrioirities[(int)Priority]++;
						I.IsDeleted = false;
						switch (Location)
						{
							case Location.Ram:
								RamEntries.AddOrUpdate(Key, Item, (key, value) => Item);
								break;
							case Location.Elasticsearch:
								await esAdd(Key, Item);
								break;
							case Location.Redis:
								rdAdd(Key, Item);
								break;
						}
					}
					else
					{
						CountsByPrioirities[(int)I.Priority]--;
						CountsByPrioirities[(int)Priority]++;
						switch (Location)
						{
							case Location.Ram:
								RamEntries[Key] = Item;
								break;
							case Location.Elasticsearch:
								await esAdd(Key, Item);
								break;
							case Location.Redis:
								rdAdd(Key, Item);
								break;
						}
					}

					I.LastUpdateTime = DateTime.UtcNow.Ticks;
					I.Location = Location;
					I.Priority = Priority;
					AddRemoveEvents(ref I, RemoveEvents);

					Index[Key] = I;
				}
				else
				{
					CountsByPrioirities[(int)Priority]++;

					I = new IndexEntry();
					I.LastUpdateTime = DateTime.UtcNow.Ticks;
					I.LastVisit = DateTime.UtcNow.Ticks;
					I.Location = Location;
					I.Priority = Priority;
					I.VisitNum = 0;
					I.ReferenceNum = 0;
					AddRemoveEvents(ref I, RemoveEvents);

					switch (Location)
					{
						case Location.Ram:
							RamEntries.AddOrUpdate(Key, Item, (key, value) => Item);
							break;
						case Location.Elasticsearch:
							await esAdd(Key, Item);
							break;
						case Location.Redis:
							rdAdd(Key, Item);
							break;
					}

					Index.AddOrUpdate(Key, I, (key, value) => I);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}



		public void RemoveGlobal(string Key)
		{
			RemoveLocal(Key);
			//CacheManager.Distributed.Client.RemoveCacheEntry(InstanceId, SubsystemId, CacheType, UserId, Name, Key);
		}


		public void RemoveGlobal(List<string> keys)
		{
			keys.ForEach(key => RemoveLocal(key));
			//CacheManager.Distributed.Client.RemoveCacheEntry(InstanceId, SubsystemId, CacheType, UserId, Name,
				//keys.ToArray());
		}


		public void RemoveLocal(string Key)
		{
			_Remove(Key);
		}


		public void RemoveGlobal<T>(Guid bizDomainId, Expression<Func<T, object>> bizdomainProperty = null)
		{
			var keys = RemoveLocal(bizDomainId, bizdomainProperty);
			//CacheManager.Distributed.Client.RemoveCacheEntry(InstanceId, SubsystemId, CacheType, UserId, Name,
				//keys.ToArray());
		}



		public List<string> RemoveLocal<T>(Guid bizDomainId, Expression<Func<T, object>> bizdomainProperty = null)
		{
			return _Remove(bizDomainId, bizdomainProperty);
		}



		public int GetCountByPriority(CachePriority Priority)
		{
			return CountsByPrioirities[(int)Priority];
		}
		public List<string> GetKeys()
		{
			var Result = new List<string>();
			Monitor.Enter(this);
			try
			{
				foreach (var Key in Index.Keys)
					if (_Contains(Key, false))
						Result.Add(Key);
			}
			finally
			{
				Monitor.Exit(this);
			}

			return Result;
		}
		public List<Entry> GetCachedEntries()
		{
			var Entries = new List<Entry>();
			foreach (var Key in Index.Keys)
			{
				var Entry = Index[Key];
				if (!Entry.IsDeleted)
					Entries.Add(new Entry(Key, Entry));
			}

			return Entries;
		}


		public void Clear()
		{
			Monitor.Enter(this);
			try
			{
				var Keys = new List<string>();
				foreach (var Key in Index.Keys)
					Keys.Add(Key);
				for (var n = 0; n < Keys.Count; n++)
					_Remove(Keys[n]);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}



		public void Lock()
		{
			Monitor.Enter(this);
		}

		public void Unlock()
		{
			Monitor.Exit(this);
		}



		public void ResetGlobalCache()
		{
			ResetLocalCache();
			//CacheManager.Distributed.Client.ResetCacheObjects(InstanceId, SubsystemId, CacheType, UserId, Name);
		}
		public void ResetLocalCache()
		{
			CacheResetEvent?.Invoke(null, null);

			Monitor.Enter(this);
			try
			{
				switch (Location)
				{
					case Location.Ram:
						RamEntries.Clear();
						break;
					case Location.Elasticsearch:
						throw new Exception("Elasticsearch caching doesn't support sync method");
					case Location.Redis:
						rdClear();
						break;
				}

				var NotDeletedKeys = new List<string>();
				foreach (var Key in Index.Keys)
					if (!Index[Key].IsDeleted)
						NotDeletedKeys.Add(Key);

				foreach (var Key in NotDeletedKeys)
				{
					var Entry = Index[Key];

					Entry.IsDeleted = true;
					Index[Key] = Entry;
				}

				for (var i = 0; i < 5; i++)
					CountsByPrioirities[i] = 0;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void ResetGlobalIndex()
		{
			ResetLocalIndex();
			//CacheManager.Distributed.Client.ResetCacheIndex(InstanceId, SubsystemId, CacheType, UserId, Name);
		}
		public void ResetLocalIndex()
		{
			Monitor.Enter(this);
			try
			{
				Index.Clear();

				var keys = new List<string>();
				switch (Location)
				{
					case Location.Ram:
						keys = RamEntries.Keys.ToList();
						break;
					case Location.Elasticsearch:
						throw new Exception("Elasticsearch caching doesn't support sync method");
					case Location.Redis:
						keys = rdGetKeys();
						break;
				}

				foreach (var Key in keys)
				{
					var Entry = new IndexEntry();

					Entry.IsDeleted = false;
					Entry.LastUpdateTime = DateTime.UtcNow.Ticks;
					Entry.LastVisit = DateTime.UtcNow.Ticks;
					Entry.Location = Location;
					Entry.Priority = CachePriority.Normal;
					Entry.ReferenceNum = 0;
					Entry.RemoveEvents = new RemoveEvent[0];
					Entry.VisitNum = 0;

					Index.AddOrUpdate(Key, Entry, (key, value) => Entry);
				}

				for (var i = 0; i < 5; i++)
					CountsByPrioirities[i] = 0;
				CountsByPrioirities[2] = keys.Count;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}


		public CacheReport GenerateReport()
		{
			Monitor.Enter(this);
			try
			{
				var count = 0;
				switch (Location)
				{
					case Location.Ram:
						count = RamEntries.Count;
						break;
					case Location.Elasticsearch:
						throw new Exception("Elasticsearch caching doesn't support sync method");
					case Location.Redis:
						count = rdCount();
						break;
				}

				var Report = new CacheReport(1, Index.Count, count, 0, 0, MaxObjectsInRam);
				foreach (var Entry in Index.Values)
				{
					Report.HitCount += Entry.VisitNum;
					Report.MissCount += Entry.ReferenceNum - Entry.VisitNum;
				}

				return Report;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		private void initialize(Guid instanceId, GenericCacheType cacheType, Guid subsystemId, Guid userId, string name,
			int maxObjectsInRam = -1,
			NumberOfEntriesToClean cleanUpHelpingMethod = null, KeysToCleanUpMethod cleanUpKeyGetter = null,
			double cleanUpRatio = -1, Location location = Location.Ram)
		{
			InstanceId = instanceId;
			CacheType = cacheType;
			SubsystemId = subsystemId;
			UserId = userId;
			Name = name;
			Location = location;
			CleanUpHelpingMethod = cleanUpHelpingMethod ?? RatioBasedCleanUpHelpingMethod;
			CleanUpKeyGetter = cleanUpKeyGetter ?? AccessBasedCleanUpMethod;
			this.maxObjectsInRam = maxObjectsInRam == -1 ? Settings.MaxObjectsInRam : maxObjectsInRam;
			this.cleanUpRatio = cleanUpRatio == -1 ? Settings.CleanUpRatio : cleanUpRatio;

			if (Location == Location.Setting)
			{
				//var appConfigReader = new AppSettingsReader();
				//var caches = appConfigReader.GetValue("RedisCache", typeof(string)).ToString().Split(',');
				//if (caches.Contains(Name))
				//	Location = Location.Redis;
				//else Location = Location.Ram;
			}

			switch (Location)
			{
				case Location.Ram:
					break;
				case Location.Elasticsearch:
					Location = Location.Ram;
					break;
				case Location.Redis:
					if (!rdClear())
						Location = Location.Ram;
					break;
			}
		}

		#region Elasticsearch

		public string esIndex => $"cache_{(string.IsNullOrEmpty(Name) ? InstanceId.ToString() : Name).ToLower()}";

		private async Task<T> esGet<T>(string key)
		{
			var response = await elastic.GetAsync($"{esIndex}/_doc/{key}");
			if (!response.IsSuccessStatusCode) return default;
			var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
			return (result["_source"] as JObject).ToObject<T>();
		}

		private async Task<bool> esExists(string key)
		{
			var response = await elastic.GetAsync($"{esIndex}/_doc/{key}");
			return response.IsSuccessStatusCode;
		}
		private async Task<bool> esRemove(params string[] keys)
		{
			if (keys.Length == 1)
				return (await elastic.DeleteAsync($"{esIndex}/_doc/{keys.First()}")).IsSuccessStatusCode;
			return (await elastic.PostAsync($"{esIndex}/_doc",
				new StringContent(
					$"{{\"query\":{{\"ids\":{{\"values\":[{string.Join(",", keys.Select(_ => $"\"{_}\""))}]}}}}}}", Encoding.UTF8,
					"application/json")
			)).IsSuccessStatusCode;
		}
		private async Task<bool> esAdd<T>(string key, T item)
		{
			var content = new StringContent(JsonConvert.SerializeObject(item, jsonSettings), Encoding.UTF8,
				"application/json");
			return (await elastic.PostAsync($"{esIndex}/_doc/{key}", content)).IsSuccessStatusCode;
		}
		private async Task<List<string>> esGetKeys(Guid? bizdomainId = null)
		{
			var content =
				$"{{{(bizdomainId.HasValue ? $"\"query\":{{\"term\":{{\"BizdomainId.keyword\":{{\"value\":\"{bizdomainId}\"}}}}}}," : "")}\"stored_fields\":[]}}";
			var response = await elastic.PostAsync($"{esIndex}/_search",
				new StringContent(content, Encoding.UTF8, "application/json"));
			var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
			return ((JArray)result.hits.hits).Select(_ => (string)_["_id"]).ToList();
		}
		#endregion

		#region Redis

		public string rdKey => (string.IsNullOrEmpty(Name) ? InstanceId.ToString() : Name).ToLower();

		private T rdGet<T>(string key)
		{
			try
			{
				var result = JsonConvert.DeserializeObject<dynamic>(redis.GetDatabase().StringGet($"{rdKey}_{key}"));
				return (result as JObject).ToObject<T>();
			}
			catch
			{
				return default;
			}
		}

		private bool rdExists(string key)
		{
			return redis.GetDatabase().KeyExists($"{rdKey}_{key}");
		}

		private void rdRemove(params string[] keys)
		{
			try
			{
				var db = redis.GetDatabase();
				foreach (var key in keys)
					db.KeyDelete($"{rdKey}_{key}");
			}
			catch
			{
			}
		}

		private bool rdAdd<T>(string key, T item, Guid? bizdomainId = null)
		{
			try
			{
				var db = redis.GetDatabase();
				return db.StringSet($"{rdKey}_{key}", JsonConvert.SerializeObject(item, jsonSettings));
			}
			catch
			{
				return false;
			}
		}

		private int rdCount()
		{
			var endpoints = redis.GetEndPoints();
			var server = redis.GetServer(endpoints.First());
			return server.Keys().Count();
		}

		private List<string> rdGetKeys(Guid? bizdomainId = null)
		{
			var endpoints = redis.GetEndPoints();
			var server = redis.GetServer(endpoints.First());
			//return server.Keys(pattern: (bizdomainId.HasValue? $"*{bizdomainId}*": null)).Select(_=> _.ToString()).ToList();
			return server.Keys().Select(_ => _.ToString()).ToList();
		}

		private bool rdClear()
		{
			try
			{
				if (redis == null) return false;
				var endpoints = redis.GetEndPoints();
				foreach (var endpoint in endpoints)
				{
					var server = redis.GetServer(endpoint);
					if (server == null) return false;
					server.FlushAllDatabases();
					//var db = redis.GetDatabase();
					//var keys = server.Keys();
					//foreach (var key in keys)
					//	db.KeyDelete(key);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion
	}

	internal class WritablePropertiesOnlyResolver : DefaultContractResolver
	{
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			return base.CreateProperties(type, memberSerialization).Where(_ => _.Writable).ToList();
		}
	}
}
