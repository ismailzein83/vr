using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    class StatusDefinitionDataManager: BaseSQLDataManager, IStatusDefinitionDataManager
    {
        #region ctor/Local Variables
        public StatusDefinitionDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<StatusDefinition> GetStatusDefinition()
        {
            return GetItemsSP("Retail.sp_StatusDefinition_GetAll", StatusDefinitionMapper);
        }

        public bool AreStatusDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.StatusDefinition", ref updateHandle);
        }
        #endregion

        #region Mappers
        StatusDefinition StatusDefinitionMapper(IDataReader reader)
        {
            StatusDefinition statusDefinition = new StatusDefinition
            {
                StatusDefinitionId = (Guid) reader["ID"],
                Name = reader["Name"] as string,
                Settings = null,
                //Settings = Vanrise.Common.Serializer.Deserialize<ServiceTypeSettings>(reader["Settings"] as string),
            }; 
            return statusDefinition;
        }
        #endregion
    }
}
