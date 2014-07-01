using System;
using System.Collections.Generic;
using FHSDK.Services;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Threading;


namespace FHSDK.Sync
{
    /// <summary>
    /// Thread-safe in memory data cache
    /// </summary>
	public class InMemoryDataStore<T> : IDataStore<T>
	{
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private Dictionary<string, T> memoryStore;
		public string PersistPath { set; get; }

		public InMemoryDataStore ()
		{
			memoryStore = new Dictionary<string, T> ();
		}

		public void Insert(string key, T obj)
		{
            cacheLock.EnterWriteLock();
            try
            {
                memoryStore [key] = obj;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
		}

		public T Get(string key)
		{
            cacheLock.EnterReadLock();
            try
            {
                T value = default(T);
                memoryStore.TryGetValue (key, out value);
                return value;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
			
		}

        public T Delete(string key)
        {
            cacheLock.EnterUpgradeableReadLock();
            try
            {
                if(memoryStore.ContainsKey(key))
                {
                    T ret = Get(key);
                    cacheLock.EnterWriteLock();
                    try
                    {
                        memoryStore.Remove(key);
                        return ret;
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                }
                return default(T);
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }

        }

		public Dictionary<string, T> List()
		{
			return memoryStore;
		}

		public void Save()
		{
			IIOService ioService = ServiceFinder.Resolve<IIOService> ();
			ILogService logger = ServiceFinder.Resolve<ILogService> ();
            Monitor.Enter(this);
            try {
                ioService.WriteFile(this.PersistPath, FHSyncUtils.SerializeObject(memoryStore));
            } catch (Exception ex) {
                logger.e ("FHSyncClient.InMemoryDataStore", "Failed to save file " + this.PersistPath, ex);
                throw ex;
            } finally {
                Monitor.Exit(this);
            }
		}

        public void Clear()
        {
            cacheLock.EnterWriteLock();
            try {
                memoryStore.Clear();
            } finally {
                cacheLock.ExitWriteLock();
            }
        }

        public IDataStore<T> Clone()
        {
            InMemoryDataStore<T> cloned = new InMemoryDataStore<T>();
            cacheLock.EnterReadLock();
            try
            {
                foreach(var entry in memoryStore){
                    cloned.Insert(entry.Key, (T)FHSyncUtils.Clone(entry.Value));
                }
                return cloned;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

        }


		public static InMemoryDataStore<X> Load<X>(string fullFilePath)
		{
			InMemoryDataStore<X> dataStore = new InMemoryDataStore<X>();
			dataStore.PersistPath = fullFilePath;
			IIOService ioService = ServiceFinder.Resolve<IIOService> ();
			ILogService logger = ServiceFinder.Resolve<ILogService> ();
			if (ioService.Exists (fullFilePath)) {
				try {
					string fileContent = ioService.ReadFile(fullFilePath);
					dataStore.memoryStore = (Dictionary<string, X>) FHSyncUtils.DeserializeObject(fileContent, typeof(Dictionary<string, X>));
				} catch (Exception ex) {
					logger.e ("FHSyncClient.InMemoryDataStore", "Failed to load file " + fullFilePath, ex);
					dataStore.memoryStore = new Dictionary<string, X> ();
				}
			}
			return dataStore;
		}
	}
}

