using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Our.Umbraco.PostgreSql.Caching
{
    /// <summary>
    /// Provides static methods for managing a collection of hashed foreign keys used for caching purposes.
    /// </summary>
    internal static class StaticCaching
    {
        private static ConcurrentBag<string>? _hashedForeignKeys;

        /// <summary>
        /// Determines whether the specified key exists in the collection of hashed foreign keys.
        /// </summary>
        /// <param name="key">The key to locate in the collection. Can be null or empty.</param>
        /// <returns>true if the collection contains the specified key; otherwise, false.</returns>
        public static bool HashedForeignKeysContains(string? key)
        {
            InitCache();

            if (_hashedForeignKeys == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty(key) && _hashedForeignKeys.Contains(key);
        }

        private static void InitCache()
        {
            if (_hashedForeignKeys == null)
            {
                _hashedForeignKeys = [];

                var persistedKeys = CachingPersister.RetrieveHashedForeignKeys();
                if (persistedKeys != null)
                {
                    foreach (var persistedKey in persistedKeys)
                    {
                        _hashedForeignKeys.Add(persistedKey);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the specified key to the collection of hashed foreign keys.
        /// </summary>
        /// <param name="key">The key to add to the hashed foreign keys collection. Cannot be null.</param>
        public static void HashedForeignKeysAdd(string key)
        {
            InitCache();

            if (_hashedForeignKeys == null)
            {
                return;
            }

            _hashedForeignKeys.Add(key);
        }

        /// <summary>
        /// Retrieves the collection of hashed foreign key values currently stored in the system.
        /// </summary>
        /// <remarks>The returned collection is safe for concurrent access and may be empty if no foreign
        /// keys have been hashed. Modifications to the returned collection do not affect the internal state.</remarks>
        /// <returns>A thread-safe collection containing the hashed foreign key strings. If no foreign keys are present, the
        /// collection will be empty.</returns>
        public static ConcurrentBag<string> GetHashedForeignKeys() =>
            _hashedForeignKeys ?? [];
    }

    internal class CachingPersister
    {
        internal static string[] RetrieveHashedForeignKeys() =>
            [

                // ToDo: is this constant on all environments?
                Constants.FkContentVersionCultureVariationAndContent,
            ];

        internal static void PersistHashedForeignKeys(IEnumerable<string> keys)
        {

        }
    }
}
