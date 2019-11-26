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
using Vanrise.Security.Entities;

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
            return InsertStatusHistory(ContextFactory.GetContext().GetLoggedInUserId(), businessEntityDefinitionId, businessEntityId, fieldName, statusId, moreInfo);
		}

        public bool InsertStatusHistory(int userId, Guid businessEntityDefinitionId, string businessEntityId, string fieldName, Guid statusId, string moreInfo = null)
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
            return dataManager.Insert(businessEntityDefinitionId, businessEntityId, fieldName, statusId, previousStatus, moreInfo, previousMoreInfo, userId);

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
				StyleDefinitionId = statusDefinitionManager.GetStyleDefinitionId(businessEntityStatusHistory.StatusId),
				PreviousStyleDefinitionId= businessEntityStatusHistory.PreviousStatusId.HasValue?statusDefinitionManager.GetStyleDefinitionId(businessEntityStatusHistory.PreviousStatusId.Value):default(Guid?),
				CreatedBy= businessEntityStatusHistory.CreatedBy,
				CreatedByDescription = businessEntityStatusHistory.CreatedBy.HasValue? BEManagerFactory.GetManager<IUserManager>().GetUserName(businessEntityStatusHistory.CreatedBy.Value):null
			};

            if (businessEntityStatusHistory.PreviousStatusId.HasValue)
                businessEntityStatusHistoryDetail.PreviousStatusName = statusDefinitionManager.GetStatusDefinitionName(businessEntityStatusHistory.PreviousStatusId.Value);

            return businessEntityStatusHistoryDetail;
        }
    }

 
}


