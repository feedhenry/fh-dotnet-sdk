using System.Collections.Generic;

namespace FHSDK.Sync
{
    /// <summary>
    /// Interfce to DataStore used in synchronization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataStore<T>
    {
        /// <summary>
        /// Where to persist.
        /// </summary>
        string PersistPath { set; get; }

        /// <summary>
        /// Add a key/value item to the store.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        void Insert(string key, T obj);

        /// <summary>
        /// Get an item from its key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get(string key);

        /// <summary>
        /// List all items.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, T> List();

        /// <summary>
        /// Delete an item from its key and return the deleted item.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Delete(string key);

        /// <summary>
        /// Reset all the storage.
        /// </summary>
        void Clear();

        /// <summary>
        /// Save the storage.
        /// </summary>
        void Save();

        /// <summary>
        /// clone the storage.
        /// </summary>
        /// <returns></returns>
        IDataStore<T> Clone();
    }
}