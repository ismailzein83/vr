using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface INationalNumberingPlanDataManager:IDataManager
    {
        List<NationalNumberingPlan> GetNationalNumberingPlans();
        bool Insert(NationalNumberingPlan operatorProfile, out int operatorProfileId);
        bool Update(NationalNumberingPlan operatorProfile);
        bool AreNationalNumberingPlansUpdated(ref object updateHandle);
    }
}
