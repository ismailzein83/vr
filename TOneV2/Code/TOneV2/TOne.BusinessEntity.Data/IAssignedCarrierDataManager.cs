using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IAssignedCarrierDataManager : IDataManager
    {
        List<AssignedCarrier> GetAssignedCarriers(List<int> userIds);

        void AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers);
    }
}
