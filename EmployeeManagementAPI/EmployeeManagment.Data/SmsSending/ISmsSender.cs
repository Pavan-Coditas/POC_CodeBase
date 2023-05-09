using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagment.Services.SmsSending
{
    public interface ISmsSender
    {
        void SendSms(string recipentNumber,string message);
    }
}
