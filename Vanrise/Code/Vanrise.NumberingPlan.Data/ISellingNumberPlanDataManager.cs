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

        bool Update(SellingNumberPlanToEdit sellingNumberPlan);

        bool Insert(SellingNumberPlan sellingNumberPlan, out int insertedId);
    }
}
