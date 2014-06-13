using System;
using System.Collections.Generic;

namespace FHSDK.Sync
{
	public interface IDataStore<T>
	{
        string PersistPath { set; get; }
		void Insert(string key, T obj);
		T Get(string key);
		Dictionary<string, T> List();
        T Delete(string key);
        void Clear();
		void Save();
	}
}

