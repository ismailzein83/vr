using System;
using System.Collections.Generic;
using System.Data;
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

        public bool Insert(Guid businessEntityDefinitionId, string businessEntityId, string fieldName, Guid statusId, Guid? previousStatusId,string moreInfo,string previousMoreInfo,int userId)
        {
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_BusinessEntityStatusHistory_Insert", businessEntityDefinitionId, businessEntityId.ToString(), fieldName, statusId,previousStatusId, moreInfo, previousMoreInfo,userId);
            return (recordesEffected > 0);
        }

        public IEnumerable<BusinessEntityStatusHistory> GetFilteredBusinessEntitiesStatusHistory(Vanrise.Entities.DataRetrievalInput<BusinessEntityStatusHistoryQuery> input)
        {
            return GetItemsSP("genericdata.sp_BusinessEntityStatusHistory_GetFiltered", BusinessEntityStatusHistoryMapper, input.Query.BusinessEntityDefinitionId, input.Query.BusinessEntityId,input.Query.FieldName);
        }


        public BusinessEntityStatusHistory GetLastBusinessEntityStatusHistory(Guid businessEntityDefinitionId, string businessEntityId, string fieldName)
        {
            return GetItemSP("genericdata.sp_BusinessEntityStatusHistory_GetLast", BusinessEntityStatusHistoryMapper, businessEntityDefinitionId,businessEntityId, fieldName);
        }

        private BusinessEntityStatusHistory BusinessEntityStatusHistoryMapper(IDataReader reader)
        {
            return new BusinessEntityStatusHistory
            {
                StatusId= GetReaderValue<Guid>(reader,"StatusId"),
                StatusChangedDate = GetReaderValue<DateTime>(reader, "StatusChangedDate"),
                FieldName = reader["FieldName"] as string,
                IsDeleted = GetReaderValue<bool>(reader, "IsDeleted"),
                PreviousStatusId = GetReaderValue<Guid?>(reader, "PreviousStatusId"),
                BusinessEntityStatusHistoryId = GetReaderValue<long>(reader, "ID"),
                BusinessEntityDefinitionId = GetReaderValue<Guid>(reader, "BusinessEntityDefinitionId"),
                BusinessEntityId = reader["BusinessEntityId"] as string,
				MoreInfo=reader["MoreInfo"] as string,
				PreviousMoreInfo = reader["PreviousMoreInfo"] as string,
				CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),

			};
        }
    }
}
