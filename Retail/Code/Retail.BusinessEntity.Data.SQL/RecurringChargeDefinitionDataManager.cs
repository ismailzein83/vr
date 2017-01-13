using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class RecurringChargeDefinitionDataManager : BaseSQLDataManager, IRecurringChargeDefinitionDataManager
    {
        #region ctor/Local Variables
        public RecurringChargeDefinitionDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<RecurringChargeDefinition> GetRecurringChargeDefinitions()
        {
            return GetItemsSP("Retail_BE.sp_RecurringChargeDefinition_GetAll", RecurringChargeDefinitionMapper);
        }

        public bool AreRecurringChargeDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.RecurringChargeDefinition", ref updateHandle);
        }

        public bool Insert(RecurringChargeDefinition recurringChargeDefinition)
        {
            string serializedSettings = recurringChargeDefinition.Settings != null ? Vanrise.Common.Serializer.Serialize(recurringChargeDefinition.Settings) : null;
            int recordesEffected = ExecuteNonQuerySP("Retail_BE.sp_RecurringChargeDefinition_Insert", recurringChargeDefinition.RecurringChargeDefinitionId, recurringChargeDefinition.Name, serializedSettings);
            return (recordesEffected > 0);
        }

        public bool Update(RecurringChargeDefinition recurringChargeDefinition)
        {
            string serializedSettings = recurringChargeDefinition.Settings != null ? Serializer.Serialize(recurringChargeDefinition.Settings) : null;

            int recordsEffected = ExecuteNonQuerySP("Retail_BE.sp_RecurringChargeDefinition_Update", recurringChargeDefinition.RecurringChargeDefinitionId, recurringChargeDefinition.Name, serializedSettings);
            return (recordsEffected > 0);
        }

        #endregion

        #region Mappers

        RecurringChargeDefinition RecurringChargeDefinitionMapper(IDataReader reader)
        {
            RecurringChargeDefinition statusDefinition = new RecurringChargeDefinition
            {
                RecurringChargeDefinitionId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                Settings = reader["Settings"] as string != null ? Vanrise.Common.Serializer.Deserialize<RecurringChargeDefinitionSettings>(reader["Settings"] as string) : null
            };
            return statusDefinition;
        }

        #endregion
    }
}
