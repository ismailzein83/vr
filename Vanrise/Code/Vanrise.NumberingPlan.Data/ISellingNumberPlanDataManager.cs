using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface ISellingNumberPlanDataManager : IDataManager
    {
        List<SellingNumberPlan> GetSellingNumberPlans();
        bool AreSellingNumberPlansUpdated(ref object updateHandle);
    }
}
