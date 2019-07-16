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
	public class BusinessEntityHistoryStackDataManager : BaseSQLDataManager,IBusinessEntityHistoryStackDataManager
	{
		public BusinessEntityHistoryStackDataManager() : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey")) { }

		public bool Insert(Guid businessEntityDefinitionId, string businessEntityId, string fieldName, Guid statusId, Guid? previousStatusId, string moreInfo, string previousMoreInfo)
		{
			int recordesEffected = ExecuteNonQuerySP("genericdata.sp_BusinessEntityHistoryStack_Insert", businessEntityDefinitionId, businessEntityId.ToString(), fieldName, statusId, previousStatusId, moreInfo, previousMoreInfo);
			return (recordesEffected > 0);
		}

		public IEnumerable<BusinessEntityHistoryStack> GetFilteredBusinessEntitiesHistoryStack(Vanrise.Entities.DataRetrievalInput<BusinessEntityHistoryStackQuery> input)
		{
			return GetItemsSP("genericdata.sp_BusinessEntityHistoryStack_GetFiltered", BusinessEntityHistoryStackMapper, input.Query.BusinessEntityDefinitionId, input.Query.BusinessEntityId);
		}


		public BusinessEntityHistoryStack GetLastBusinessEntityHistoryStack(Guid businessEntityDefinitionId, string businessEntityId, string fieldName)
		{
			return GetItemSP("genericdata.sp_BusinessEntityHistoryStack_GetLast", BusinessEntityHistoryStackMapper, businessEntityDefinitionId, businessEntityId, fieldName);
		}

		public bool DeleteBusinessEntityHistoryStack(long id)
		{
			int recordesEffected = ExecuteNonQuerySP("genericdata.sp_BusinessEntityHistoryStack_Delete", id);
			return (recordesEffected > 0);
		}
		private BusinessEntityHistoryStack BusinessEntityHistoryStackMapper(IDataReader reader)
		{
			return new BusinessEntityHistoryStack
			{
				StatusId = GetReaderValue<Guid>(reader, "StatusId"),
				StatusChangedDate = GetReaderValue<DateTime>(reader, "StatusChangedDate"),
				FieldName = reader["FieldName"] as string,
				IsDeleted = GetReaderValue<bool>(reader, "IsDeleted"),
				PreviousStatusId = GetReaderValue<Guid?>(reader, "PreviousStatusId"),
				BusinessEntityHistoryStackId = GetReaderValue<long>(reader, "ID"),
				BusinessEntityDefinitionId = GetReaderValue<Guid>(reader, "BusinessEntityDefinitionId"),
				BusinessEntityId = reader["BusinessEntityId"] as string,
				MoreInfo = reader["MoreInfo"] as string,
				PreviousMoreInfo = reader["PreviousMoreInfo"] as string
			};
		}


	}
}
