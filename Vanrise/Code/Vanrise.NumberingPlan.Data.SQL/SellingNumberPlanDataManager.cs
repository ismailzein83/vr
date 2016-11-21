using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data.SQL
{
    public class SellingNumberPlanDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISellingNumberPlanDataManager
    {

        #region ctor/Local Variables
        public SellingNumberPlanDataManager()
            : base(GetConnectionStringName("NumberingPlanDBConnStringKey", "NumberingPlanDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<SellingNumberPlan> GetSellingNumberPlans()
        {
            return GetItemsSP("VR_NumberingPlan.sp_SellingNumberPlan_GetAll", SellingNumberPlanMapper);

        }
        public bool AreSellingNumberPlansUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VR_NumberingPlan.SellingNumberPlan", ref updateHandle);
        }
        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        SellingNumberPlan SellingNumberPlanMapper(IDataReader reader)
        {
            SellingNumberPlan sellingNumberPlan = new SellingNumberPlan
            {
                SellingNumberPlanId = (int)reader["ID"],
                Name = reader["Name"] as string,
            };
            return sellingNumberPlan;
        }
        #endregion

    }
}
