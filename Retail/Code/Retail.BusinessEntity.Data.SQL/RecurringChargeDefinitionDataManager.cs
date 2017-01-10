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
            return GetItemsSP("Retail_BE.sp_RecurringDefinition_GetAll", RecurringChargeDefinitionMapper);
        }

        public bool AreRecurringChargeDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.RecurringDefinition", ref updateHandle);
        }

        public bool Insert(RecurringChargeDefinition recurringChargeDefinition)
        {
            throw new NotImplementedException();
        }

        public bool Update(RecurringChargeDefinition recurringChargeDefinition)
        {
            throw new NotImplementedException();
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
