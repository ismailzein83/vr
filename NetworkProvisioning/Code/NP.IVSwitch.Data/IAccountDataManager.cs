using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
    public interface IAccountDataManager : IDataManager
    {
        bool Insert(Account account, out int insertedId);
        void UpdateCustomerChannelLimit(int accountId);
    }
}
