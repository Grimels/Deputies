using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Deputies.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task InsertMany(IList<TEntity> entities);

        Task Insert(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(string id);

        Task Clear();

        Task<IList<TEntity>> SearchFor(Func<TEntity, bool> predicate, int? limit = null, int? offset = null, Func<TEntity, object> orderBy = null, bool? asc = null);

        Task<IList<TEntity>> GetAll();

        Task<TEntity> GetById(string id);

        Task<long> Count();

        Task<long> Count(Func<TEntity, bool> predicate);
    }
}
