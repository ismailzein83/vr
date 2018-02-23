using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class BusinessEntityStatusHistoryDataManager:BaseSQLDataManager,IBusinessEntityStatusHistoryDataManager
    {
        public BusinessEntityStatusHistoryDataManager() : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey")) { }
  
        public bool Insert(Guid businessEntityDefinitionId, object businessEntityId, string fieldName, Guid statusId, Guid? previousStatusId)
        {
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_BusinessEntityStatusHistory_Insert", businessEntityDefinitionId, businessEntityId.ToString(), fieldName, statusId,previousStatusId);
            return (recordesEffected > 0);
        }
    }
}
