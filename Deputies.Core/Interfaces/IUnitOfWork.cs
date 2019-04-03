using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
    }
}
