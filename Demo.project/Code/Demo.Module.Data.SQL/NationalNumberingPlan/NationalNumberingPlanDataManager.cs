using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class NationalNumberingPlanDataManager : BaseSQLDataManager, INationalNumberingPlanDataManager
    {
   
        #region ctor/Local Variables
        public NationalNumberingPlanDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(NationalNumberingPlan plan, out int insertedId)
        {
            object planId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_NationalNumberingPlan_Insert", out planId, plan.OperatorId, plan.FromDate, plan.ToDate, Vanrise.Common.Serializer.Serialize(plan.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)planId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(NationalNumberingPlan plan)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_NationalNumberingPlan_Update", plan.NationalNumberingPlanId, plan.OperatorId, plan.FromDate, plan.ToDate, Vanrise.Common.Serializer.Serialize(plan.Settings));
            return (recordsEffected > 0);
        }
        public bool AreNationalNumberingPlansUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.NationalNumberingPlan", ref updateHandle);
        }
        public List<NationalNumberingPlan> GetNationalNumberingPlans()
        {
            return GetItemsSP("dbo.sp_NationalNumberingPlan_GetAll", NationalNumberingPlanMapper);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private NationalNumberingPlan NationalNumberingPlanMapper(IDataReader reader)
        {
            NationalNumberingPlan plan = new NationalNumberingPlan
            {
                NationalNumberingPlanId = (int)reader["ID"],
                OperatorId = (int)reader["OperatorID"],
                FromDate = GetReaderValue<DateTime?>(reader,"FromDate"),
                ToDate = GetReaderValue<DateTime?>(reader,"ToDate"),
                Settings = Vanrise.Common.Serializer.Deserialize<NationalNumberingPlanSettings>(reader["Settings"] as string)
            };
            return plan;
        }

        #endregion
      
    }
}
