using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Blazor.Gitter.Library.Services
{
    public abstract class CachedRepository<T>
    {
        MemoryCache _Cache;

        public CachedRepository()
        {
            _Cache = new MemoryCache(new MemoryCacheOptions());
        }

        protected abstract string _MakeKey(T item);

        public async Task AddOrUpdateRangeAsync(IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                await AddOrUpdateAsync(item);
            }

            Console.WriteLine($"User cache count: {_Cache.Count}");
        }

        private async Task AddOrUpdateAsync(T item)
        {
            await _Cache.GetOrCreateAsync(_MakeKey(item), entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return Task.FromResult(item);
            });
        }

        protected IReadOnlyCollection<T> _GetCurrentEntries()
        {
            // UGLY: approaching this with reflection isn't great.

            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(_Cache) as ICollection;

            var entries = new List<T>();

            PropertyInfo kvpValueProperty = null;

            if (collection != null)
                foreach (var item in collection)
                {
                    if (kvpValueProperty == null)
                        kvpValueProperty = item.GetType().GetProperty("Value");

                    // TODO MAYBE: return newest entries first?

                    if ((kvpValueProperty.GetValue(item) as ICacheEntry)?.Value is T value)
                        entries.Add(value);
                }

            return entries.AsReadOnly();
        }
    }

    /// <summary>
    /// Stores a local cache of known <see cref="IChatUser"/> instances. It gets
    /// filled whenever new messages come in, users are queried, etc., and has a
    /// sliding expiration.
    ///
    /// This is done mainly to work around
    /// <see cref="GitterApi.GetChatRoomUsers(string)"/> being unreliable.
    /// </summary>
    public class RoomUsersRepository : CachedRepository<IChatUser>
    {
        private readonly IChatApi _GitterApi;

        public RoomUsersRepository(IChatApi gitterApi)
            => _GitterApi = gitterApi;

        /// <summary>
        /// Query the cache and the API (in turn adding to the cache) for a
        /// user by partial <see cref="IChatUser.UserName" /> or
        /// <see cref="IChatUser.DisplayName" />.
        ///
        /// If the query is empty, we just display some of the cache.
        /// </summary>
        /// <param name="onlyQueryTheCache">Skip querying the API</param>
        public async Task<IEnumerable<IChatUser>> QueryAsync(IChatRoom room, string query, bool onlyQueryTheCache = false)
        {
            // BUG: room is actually ignored for cache. If you're in multiple
            // rooms, this will suggest users who aren't here.

            if (!onlyQueryTheCache && !(string.IsNullOrWhiteSpace(query)))
            {
                var users = await _GitterApi.GetChatRoomUsers(room.Id, new GitterRoomUserOptions { Query = query });

                await AddOrUpdateRangeAsync(users);
            }

            return _GetCurrentEntries().Where(u => u.Username.Contains(query, StringComparison.OrdinalIgnoreCase) || u.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        protected override string _MakeKey(IChatUser item)
            => $"User_{item.Id}";
    }
}
