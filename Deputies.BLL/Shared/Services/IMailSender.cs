using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Shared.Services
{
    public interface IMailSender
    {
        void Send(string inputEmail, string subject, string body);
    }
}
