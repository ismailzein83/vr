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

        public bool Update(Entities.SellingNumberPlanToEdit sellingNumberPlan)
        {
            int recordsEffected = ExecuteNonQuerySP("[VR_NumberingPlan].[sp_SellingNumberPlan_Update]", sellingNumberPlan.SellingNumberPlanId, sellingNumberPlan.Name);
            return (recordsEffected > 0);
        }
        public bool Insert(Entities.SellingNumberPlan sellingNumberPlan, out int insertedId)
        {
            object sellingNumberPlanId;
            int recordsEffected = ExecuteNonQuerySP("[VR_NumberingPlan].[sp_SellingNumberPlan_Insert]", out sellingNumberPlanId, sellingNumberPlan.Name);
            insertedId = (int)sellingNumberPlanId;
            return (recordsEffected > 0);
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
