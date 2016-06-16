using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class SwitchDataManager :BaseSQLDataManager, ISwitchDataManager
    {
           
        #region ctor/Local Variables
        public SwitchDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "Retail_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public IEnumerable<Switch> GetSwitches()
        {
            return GetItemsSP("Retail.sp_Switch_GetAll", SwitchMapper);
        }

        public bool AreSwitchUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.ServiceType", ref updateHandle);
        }


        public bool Insert(Switch switchItem, out int insertedId)
        {
            object switchId;
            string serializedSettings = switchItem.Settings != null ? Vanrise.Common.Serializer.Serialize(switchItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Switch_Insert", out switchId, switchItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                insertedId = (int)switchId;
                return true;
            }

            insertedId = -1;
            return false;
        }


        public bool Update(Switch switchItem)
        {
            string serializedSettings = switchItem.Settings != null ? Vanrise.Common.Serializer.Serialize(switchItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Switch_Update", switchItem.SwitchId, switchItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }


        #endregion

        #region Private Methods

        #endregion

        #region  Mappers

        private Switch SwitchMapper(IDataReader reader)
        {
            return new Switch()
            {
                SwitchId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<SwitchSettings>(reader["Settings"] as string),
            };
        }

        #endregion
    }
}
