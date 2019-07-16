using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
	public interface IBusinessEntityHistoryStackDataManager : IDataManager
	{
		bool Insert(Guid businessEntityDefinitionId, string businessEntityId, string fieldName, Guid statusId, Guid? previousStatusId, string moreInfo, string previousMoreInfo);
		BusinessEntityHistoryStack GetLastBusinessEntityHistoryStack(Guid businessEntityDefinitionId, string businessEntityId, string fieldName);
		IEnumerable<BusinessEntityHistoryStack> GetFilteredBusinessEntitiesHistoryStack(Vanrise.Entities.DataRetrievalInput<BusinessEntityHistoryStackQuery> input);
		bool DeleteBusinessEntityHistoryStack(long id);
	}
}
