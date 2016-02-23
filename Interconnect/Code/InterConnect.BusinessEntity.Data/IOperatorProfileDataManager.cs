using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterConnect.BusinessEntity.Data
{
    public interface IOperatorProfileDataManager:IDataManager
    {
        bool Insert(OperatorProfile operatorProfile, out int insertedId);
        bool Update(OperatorProfile operatorProfile);
        bool AreOperatorProfilesUpdated(ref object updateHandle);
        List<OperatorProfile> GetOperatorProfiles();
    }
}
