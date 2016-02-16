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
        public bool Insert(NationalNumberingPlan operatorProfile, out int insertedId)
        {
            object operatorProfileId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_NationalNumberingPlan_Insert", out operatorProfileId, operatorProfile.Name, Vanrise.Common.Serializer.Serialize(operatorProfile.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)operatorProfileId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(NationalNumberingPlan operatorProfile)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_NationalNumberingPlan_Update", operatorProfile.NationalNumberingPlanId, operatorProfile.Name, Vanrise.Common.Serializer.Serialize(operatorProfile.Settings));
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
            NationalNumberingPlan operatorProfile = new NationalNumberingPlan
            {
                NationalNumberingPlanId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<NationalNumberingPlanSettings>(reader["Settings"] as string)
            };
            return operatorProfile;
        }

        #endregion
      
    }
}
