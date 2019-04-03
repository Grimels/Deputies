using Deputies.Core;
using Deputies.Core.Interfaces;
using MongoDB.Driver;
using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace Deputies.DAL.Mongo
{
    public class MongoRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> collection;

        public MongoRepository()
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("inqury");
            this.collection = database.GetCollection<T>(typeof(T).Name);
        }

        public async Task Clear()
        {
            await this.collection.DeleteManyAsync(x => true);
        }

        public async Task<long> Count()
        {
            return await this.collection.CountDocumentsAsync(x => true);
        }

        public async Task<long> Count(Func<T, bool> predicate)
        {
            await UploadToCacheIfNeededAsync();
            return GetFromCache().Count<T>(predicate);
        }

        public async Task Delete(string id)
        {
            await this.collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<IList<T>> GetAll()
        {
            await UploadToCacheIfNeededAsync();
            return GetFromCache();
        }

        public async Task<T> GetById(string id)
        {
            await UploadToCacheIfNeededAsync();
            return GetFromCache().FirstOrDefault(x => x.Id == id);
        }

        public async Task Insert(T entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await this.collection.InsertOneAsync(entity);
        }

        public async Task<IList<T>> SearchFor(Func<T, bool> predicate, int? limit = null, int? offset = null, Func<T, object> orderBy = null, bool? asc = null)
        {
            await UploadToCacheIfNeededAsync();
            if (limit == null || offset == null || asc == null || orderBy == null)
            {
                return GetFromCache().Where(predicate).ToList();
            }

            if (asc.Value)
            {
                return GetFromCache().Where(predicate).OrderBy(orderBy).Skip(offset.Value).Take(limit.Value).ToList();
            }

            return GetFromCache().Where(predicate).OrderByDescending(orderBy).Skip(offset.Value).Take(limit.Value).ToList();
        }

        public async Task Update(T entity)
        {
            await this.collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, new UpdateOptions
            {
                IsUpsert = true
            });
        }

        public async Task InsertMany(IList<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid().ToString();
            }

            await this.collection.InsertManyAsync(entities);
        }

        private async Task UploadToCacheIfNeededAsync()
        {
            var cache = this.GetFromCache();
            if (cache == null)
            {
                var allEntities = await this.collection.Find(x => true).ToListAsync();
                MemoryCache.Default.Set(typeof(T).FullName, allEntities, new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddDays(1))
                });
            }
        }

        private IList<T> GetFromCache()
        {
            return MemoryCache.Default.Get(typeof(T).FullName) as IList<T>;
        }
    }
}
