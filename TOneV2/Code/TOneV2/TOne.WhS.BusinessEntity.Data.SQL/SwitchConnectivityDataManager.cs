using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SwitchConnectivityDataManager : BaseSQLDataManager, ISwitchConnectivityDataManager
    {
        #region Constructors

        public SwitchConnectivityDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<SwitchConnectivity> GetSwitchConnectivities()
        {
            return GetItemsSP("TOneWhS_BE.sp_SwitchConnectivity_GetAll", SwitchConnectivityMapper);
        }

        public bool AreSwitchConnectivitiesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SwitchConnectivity", ref updateHandle);
        }

        public bool Insert(SwitchConnectivity switchConnectivity, out int insertedId)
        {
            object switchConnectivityId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SwitchConnectivity_Insert", out switchConnectivityId, switchConnectivity.Name, switchConnectivity.SwitchId, switchConnectivity.CarrierAccountId, Vanrise.Common.Serializer.Serialize(switchConnectivity.Settings), switchConnectivity.BED, switchConnectivity.EED, switchConnectivity.CreatedBy, switchConnectivity.LastModifiedBy);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)switchConnectivityId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }

        public bool Update(SwitchConnectivityToEdit switchConnectivity)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SwitchConnectivity_Update", switchConnectivity.SwitchConnectivityId, switchConnectivity.Name, switchConnectivity.SwitchId, switchConnectivity.CarrierAccountId, Vanrise.Common.Serializer.Serialize(switchConnectivity.Settings), switchConnectivity.BED, switchConnectivity.EED, switchConnectivity.LastModifiedBy);
            return (recordsEffected > 0);
        }
        
        #endregion

        #region  Mappers

        private SwitchConnectivity SwitchConnectivityMapper(IDataReader reader)
        {
            SwitchConnectivity switchConnectivity = new SwitchConnectivity
            {
                SwitchConnectivityId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SwitchId= GetReaderValue<int>(reader,"SwitchId"),
                Settings = Vanrise.Common.Serializer.Deserialize<SwitchConnectivitySettings>(reader["Settings"] as string),
                CarrierAccountId = GetReaderValue<int>(reader, "CarrierAccountID"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime"),
            };
            return switchConnectivity;
        }

        #endregion
    }
}
