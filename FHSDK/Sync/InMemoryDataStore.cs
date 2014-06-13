using System;
using System.Collections.Generic;
using FHSDK.Services;
using System.Diagnostics.Contracts;
using System.Collections;


namespace FHSDK.Sync
{
	public class InMemoryDataStore<T> : IDataStore<T>
	{
		private Dictionary<string, T> memoryStore;
		public string PersistPath { set; get; }

		public InMemoryDataStore ()
		{
			memoryStore = new Dictionary<string, T> ();
		}

		public void Insert(string key, T obj)
		{
			memoryStore [key] = obj;
		}

		public T Get(string key)
		{
			T value = default(T);
			memoryStore.TryGetValue (key, out value);
			return value;
		}

        public T Delete(string key)
        {
            if(memoryStore.ContainsKey(key))
            {
                T ret = Get(key);
                memoryStore.Remove(key);
                return ret;
            }
            return default(T);
        }

		public Dictionary<string, T> List()
		{
			return memoryStore;
		}

		public void Save()
		{
			Contract.Assert (null != this.PersistPath, "No persist path specified!");
			IIOService ioService = ServiceFinder.Resolve<IIOService> ();
			ILogService logger = ServiceFinder.Resolve<ILogService> ();
			try {
				ioService.WriteFile(this.PersistPath, FHSyncUtils.SerializeObject(memoryStore));
			} catch (Exception ex) {
				logger.e ("FHSyncClient.InMemoryDataStore", "Failed to save file " + this.PersistPath, ex);
				throw ex;
			}
		}

        public void Clear()
        {
            memoryStore.Clear();
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
					dataStore.memoryStore = (Dictionary<string, X>) FHSyncUtils.DeserializeObject(fileContent, typeof(X));
				} catch (Exception ex) {
					logger.e ("FHSyncClient.InMemoryDataStore", "Failed to load file " + fullFilePath, ex);
					dataStore.memoryStore = new Dictionary<string, X> ();
				}
			}
			return dataStore;
		}
	}
}

