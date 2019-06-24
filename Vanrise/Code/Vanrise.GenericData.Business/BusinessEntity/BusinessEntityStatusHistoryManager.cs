using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.Common.Business;

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
        public Vanrise.Entities.IDataRetrievalResult<BusinessEntityStatusHistoryDetail> GetFilteredBusinessEntitiesStatusHistory(Vanrise.Entities.DataRetrievalInput<BusinessEntityStatusHistoryQuery> input)
        {
            IBusinessEntityStatusHistoryDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityStatusHistoryDataManager>();
            IEnumerable<BusinessEntityStatusHistory> businessEntitiesStatusHistory = dataManager.GetFilteredBusinessEntitiesStatusHistory(input);

            Func<BusinessEntityStatusHistory, bool> filterExpression = (item) =>
            {
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, businessEntitiesStatusHistory.ToBigResult(input, filterExpression, BusinessEntityStatusHistoryDetailMapper));
        }
        
        BusinessEntityStatusHistoryDetail BusinessEntityStatusHistoryDetailMapper(BusinessEntityStatusHistory businessEntityStatusHistory)
        {
            var statusDefinitionManager = new StatusDefinitionManager();

            var businessEntityStatusHistoryDetail = new BusinessEntityStatusHistoryDetail
            {
                BusinessEntityStatusHistoryId = businessEntityStatusHistory.BusinessEntityStatusHistoryId,
                FieldName = businessEntityStatusHistory.FieldName,
                StatusName = statusDefinitionManager.GetStatusDefinitionName(businessEntityStatusHistory.StatusId),
                StatusChangedDate = businessEntityStatusHistory.StatusChangedDate,
                IsDeleted = businessEntityStatusHistory.IsDeleted,
            };

            if (businessEntityStatusHistory.PreviousStatusId.HasValue)
                businessEntityStatusHistoryDetail.PreviousStatusName = statusDefinitionManager.GetStatusDefinitionName(businessEntityStatusHistory.PreviousStatusId.Value);

            return businessEntityStatusHistoryDetail;
        }
    }

 
}


