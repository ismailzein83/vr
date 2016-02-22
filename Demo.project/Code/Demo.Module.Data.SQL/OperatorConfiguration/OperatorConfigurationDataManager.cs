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
    public class OperatorConfigurationDataManager : BaseSQLDataManager, IOperatorConfigurationDataManager
    {

        #region ctor/Local Variables
        public OperatorConfigurationDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(OperatorConfiguration config, out int insertedId)
        {
            object infoId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_OperatorConfiguration_Insert", out infoId, config.OperatorId, config.Volume, config.AmountType, config.CDRType, config.CDRDirection, config.UnitType, config.Percentage, config.Amount, config.Currency);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)infoId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(OperatorConfiguration config)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_OperatorConfiguration_Update", config.OperatorConfigurationId, config.OperatorId, config.Volume, config.AmountType, config.CDRType, config.CDRDirection, config.UnitType, config.Percentage, config.Amount, config.Currency);
            return (recordsEffected > 0);
        }
        public bool AreOperatorConfigurationsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.OperatorConfiguration", ref updateHandle);
        }
        public List<OperatorConfiguration> GetOperatorConfigurations()
        {
            return GetItemsSP("dbo.sp_OperatorConfiguration_GetAll", OperatorConfigurationMapper);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private OperatorConfiguration OperatorConfigurationMapper(IDataReader reader)
        {
            OperatorConfiguration config = new OperatorConfiguration
            {
                OperatorConfigurationId = (int)reader["ID"],
                OperatorId = (int)reader["OperatorID"],
                Volume = GetReaderValue<int>(reader, "Volume"),
                AmountType = GetReaderValue<int>(reader, "AmountType"),
                CDRType = GetReaderValue<Demo.Module.Entities.CDRType>(reader, "CDRType"),
                CDRDirection = GetReaderValue<CDRDirection>(reader, "CDRDirection"),
                UnitType = (int)reader["UnitType"],
                Percentage = GetReaderValue<double?>(reader, "Percentage"),
                Amount = GetReaderValue<double?>(reader, "Amount"),
                Currency = GetReaderValue<int?>(reader, "Currency"),
            };
            return config;
        }

        #endregion

    }
}
