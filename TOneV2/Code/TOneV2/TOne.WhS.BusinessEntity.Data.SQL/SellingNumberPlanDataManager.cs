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
       
        #region ctor/Local Variables
        public SellingNumberPlanDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public bool Update(Entities.SellingNumberPlan sellingNumberPlan)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_SellingNumberPlan_Update]", sellingNumberPlan.SellingNumberPlanId, sellingNumberPlan.Name);
            return (recordsEffected > 0);
        }
        public bool Insert(Entities.SellingNumberPlan sellingNumberPlan, out int insertedId)
        {
            object sellingNumberPlanId;
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_SellingNumberPlan_Insert]", out sellingNumberPlanId, sellingNumberPlan.Name);
            insertedId = (int)sellingNumberPlanId;
            return (recordsEffected > 0);
        }
        public List<SellingNumberPlan> GetSellingNumberPlans()
        {
            return GetItemsSP("TOneWhS_BE.sp_SellingNumberPlan_GetAll", SellingNumberPlanMapper);

        }
        public bool AreSellingNumberPlansUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SellingNumberPlan", ref updateHandle);
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
