using Deputies.BLL.Features.Index.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Index.Services
{
    public interface IIndexService
    {
        Task<IndexModel> GetIndexModelAsync();
    }
}
