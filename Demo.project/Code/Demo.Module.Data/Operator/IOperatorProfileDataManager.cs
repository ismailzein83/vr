using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface IOperatorProfileDataManager:IDataManager
    {
        List<OperatorProfile> GetOperatorProfiles();
        bool Insert(OperatorProfile operatorProfile, out int operatorProfileId);
        bool Update(OperatorProfile operatorProfile);
        bool AreOperatorProfilesUpdated(ref object updateHandle);
    }
}
