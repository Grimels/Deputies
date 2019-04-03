using Deputies.Core.Interfaces;
using MongoDB.Driver;
using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.DAL.Mongo
{
    public class MongoRepositoryWithoutCache<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> collection;

        public MongoRepositoryWithoutCache()
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
            return await this.collection.CountAsync(x => true);
        }

        public async Task<long> Count(Expression<Func<T, bool>> predicate)
        {
            return await this.collection.CountAsync(predicate);
        }

        public async Task Delete(string id)
        {
            await this.collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<IList<T>> GetAll()
        {
            return await this.collection.Find(x => true).ToListAsync();
        }

        public async Task<T> GetById(string id)
        {
            return await this.collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Insert(T entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await this.collection.InsertOneAsync(entity);
        }

        public async Task<IList<T>> SearchFor(Expression<Func<T, bool>> predicate, int? limit = null, int? offset = null, Expression<Func<T, object>> orderBy = null, bool? asc = null)
        {
            if (limit == null || offset == null || asc == null || orderBy == null)
            {
                return await this.collection.Find(predicate).ToListAsync();
            }

            if (asc.Value)
            {
                return await this.collection.Find(predicate).SortBy(orderBy).Limit(limit).Skip(offset).ToListAsync();
            }

            return await this.collection.Find(predicate).SortByDescending(orderBy).Limit(limit).Skip(offset).ToListAsync();
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
    }
}
