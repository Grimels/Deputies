using Deputies.Core.Interfaces;
using ParliamentaryInquiry.Core.Entities;

namespace Deputies.DAL.Mongo
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            return new MongoRepository<T>();
        }
    }
}
