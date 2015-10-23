using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SellingNumberPlanDataManager : BaseTOneDataManager, ISellingNumberPlanDataManager
    {
        public SellingNumberPlanDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }


        public List<SellingNumberPlan> GetSellingNumberPlans()
        {
            return GetItemsSP("TOneWhS_BE.sp_SellingNumberPlan_GetAll", SellingNumberPlanMapper);
           
        }

        SellingNumberPlan SellingNumberPlanMapper(IDataReader reader)
        {
            SellingNumberPlan sellingNumberPlan = new SellingNumberPlan
            {
                SellingNumberPlanId = (int)reader["ID"],
                Name = reader["Name"] as string,
            };
            return sellingNumberPlan;
        }


        public bool AreSellingNumberPlansUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SellingNumberPlan", ref updateHandle);
        }
    }
}
