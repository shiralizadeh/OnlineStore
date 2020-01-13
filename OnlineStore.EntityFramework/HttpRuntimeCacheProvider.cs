using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace OnlineStore.EntityFramework
{
    public static class HttpRuntimeCacheProvider
    {
        #region Methods (6)

        // Public Methods (2) 

        public static IList<TEntity> ToCacheableList<TEntity>(
                            this IQueryable<TEntity> query,
                            int durationMinutes = 15,
                            CacheItemPriority priority = CacheItemPriority.Normal)
        {
            return query.Cacheable(x => x.ToList(), durationMinutes, priority);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// The cache entry has a one minute sliding expiration with normal priority.
        /// </summary>
        public static TResult Cacheable<TEntity, TResult>(
                            this IQueryable<TEntity> query,
                            Func<IQueryable<TEntity>, TResult> materializer,
                            int durationMinutes = 15,
                            CacheItemPriority priority = CacheItemPriority.Normal)
        {
            // Gets a cache key for a query.
            var queryCacheKey = query.GetCacheKey();

            // The name of the cache key used to clear the cache. All cached items depend on this key.
            var rootCacheKey = typeof(TEntity).FullName;

            // Try to get the query result from the cache.
            printAllCachedKeys();
            var result = HttpRuntime.Cache.Get(queryCacheKey);
            if (result != null)
            {
                debugWriteLine("Fetching object '{0}__{1}' from the cache.", rootCacheKey, queryCacheKey);
                return (TResult)result;
            }

            // Materialize the query.
            result = materializer(query);

            // Adding new data.
            debugWriteLine("Adding new data: queryKey={0}, dependencyKey={1}", queryCacheKey, rootCacheKey);
            storeRootCacheKey(rootCacheKey);
            HttpRuntime.Cache.Insert(
                    key: queryCacheKey,
                    value: result,
                    dependencies: new CacheDependency(null, new[] { rootCacheKey }),
                    absoluteExpiration: DateTime.Now.AddMinutes(durationMinutes),
                    slidingExpiration: Cache.NoSlidingExpiration,
                    priority: priority,
                    onRemoveCallback: null);

            return (TResult)result;
        }

        /// <summary>
        /// Call this method in `public override int SaveChanges()` of your DbContext class 
        /// to Invalidate Second Level Cache automatically.
        /// </summary>        
        public static void InvalidateSecondLevelCache(this DbContext ctx)
        {
            var changedEntityNames = ctx.ChangeTracker
                                      .Entries()
                                      .Where(x => x.State == EntityState.Added ||
                                                  x.State == EntityState.Modified ||
                                                  x.State == EntityState.Deleted)
                                      .Select(x => ObjectContext.GetObjectType(x.Entity.GetType()).FullName)
                                      .Distinct()
                                      .ToList();

            if (!changedEntityNames.Any()) return;

            printAllCachedKeys();
            foreach (var item in changedEntityNames)
            {
                item.removeEntityCache();
            }
            printAllCachedKeys();
        }
        // Private Methods (4) 

        private static void debugWriteLine(string format, params object[] args)
        {
            if (!Debugger.IsAttached) return;
            Debug.WriteLine(format, args);
        }

        private static void printAllCachedKeys()
        {
            if (!Debugger.IsAttached) return;
            debugWriteLine("Available cached keys list:");
            int count = 0;
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().StartsWith("__")) continue; // such as __System.Web.WebPages.Deployment
                debugWriteLine("queryKey: {0}", enumerator.Key.ToString());
                count++;
            }
            debugWriteLine("count: {0}", count);
        }

        private static void removeEntityCache(this string rootCacheKey)
        {
            if (string.IsNullOrWhiteSpace(rootCacheKey)) return;
            debugWriteLine("Removing items with dependencyKey={0}", rootCacheKey);
            // Removes all cached items depend on this key.
            HttpRuntime.Cache.Remove(rootCacheKey);
        }

        private static void storeRootCacheKey(string rootCacheKey)
        {
            // The cacheKeys of a cacheDependency that are not already in cache ARE NOT inserted into the cache 
            // on the Insert of the item in which the dependency is used.
            if (HttpRuntime.Cache.Get(rootCacheKey) != null)
                return;

            HttpRuntime.Cache.Add(
                rootCacheKey,
                rootCacheKey,
                null,
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration,
                CacheItemPriority.Default,
                null);
        }

        #endregion Methods
    }
}