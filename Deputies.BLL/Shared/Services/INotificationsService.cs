using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Shared.Services
{
    public interface INotificationsService
    {
        Task Subscribe(string email, string deputyId);

        Task Unsubscribe(string email);
    }
}
