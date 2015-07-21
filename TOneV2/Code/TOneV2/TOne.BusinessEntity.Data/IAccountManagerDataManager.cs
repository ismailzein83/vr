using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IAccountManagerDataManager : IDataManager
    {
        List<AccountManagerCarrier> GetCarriers(int from, int to);
        List<AssignedAccountManagerCarrier> GetAssignedCarriers(int userId);
        void AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers);
    }
}
