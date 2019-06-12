using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.GenericData.Business
{
	public struct BEDefinition
	{
		public Guid BusinessEntityDefinitionId { get; set; }
		public Object BusinessEntityId { get; set; }
		public string FieldName { get; set; }
	}

	public class BusinessEntityStatusHistoryManager
	{
		public bool InsertStatusHistory(Guid businessEntityDefinitionId, string businessEntityId, string fieldName, Guid statusId, string moreInfo = null)
		{

			var lastStatusHistory = GetLastBusinessEntityStatusHistory(businessEntityDefinitionId, businessEntityId, fieldName);
			Guid? previousStatus = null;
			string previousMoreInfo = null;
			if (lastStatusHistory != null)
			{
				if (lastStatusHistory.StatusId == statusId)
					return true;
				previousStatus = lastStatusHistory.StatusId;
				previousMoreInfo = lastStatusHistory.MoreInfo;
			}
			IBusinessEntityStatusHistoryDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityStatusHistoryDataManager>();
			return dataManager.Insert(businessEntityDefinitionId, businessEntityId, fieldName, statusId, previousStatus, moreInfo, previousMoreInfo);

		}
		public BusinessEntityStatusHistory GetLastBusinessEntityStatusHistory(Guid businessEntityDefinitionId, string businessEntityId, string fieldName)
		{
			IBusinessEntityStatusHistoryDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityStatusHistoryDataManager>();
			return dataManager.GetLastBusinessEntityStatusHistory(businessEntityDefinitionId, businessEntityId, fieldName);
		}
	}
}
