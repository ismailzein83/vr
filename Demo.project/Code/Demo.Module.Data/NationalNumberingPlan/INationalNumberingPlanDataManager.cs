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
        bool Insert(NationalNumberingPlan plan, out int planId);
        bool Update(NationalNumberingPlan plan);
        bool AreNationalNumberingPlansUpdated(ref object updateHandle);
    }
}
