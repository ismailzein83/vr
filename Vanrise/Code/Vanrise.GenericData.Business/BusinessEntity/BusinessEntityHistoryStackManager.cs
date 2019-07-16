using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
	public class BusinessEntityHistoryStackManager
	{
		public bool InsertHistoryStack(Guid businessEntityDefinitionId, string businessEntityId, string fieldName, Guid statusId, string moreInfo = null)
		{

			var lastStatusHistory = GetLastBusinessEntityHistoryStack(businessEntityDefinitionId, businessEntityId, fieldName);
			Guid? previousStatus = null;
			string previousMoreInfo = null;
			if (lastStatusHistory != null)
			{
				if (lastStatusHistory.StatusId == statusId)
					return true;
				previousStatus = lastStatusHistory.StatusId;
				previousMoreInfo = lastStatusHistory.MoreInfo;
			}
			IBusinessEntityHistoryStackDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityHistoryStackDataManager>();
			return dataManager.Insert(businessEntityDefinitionId, businessEntityId, fieldName, statusId, previousStatus, moreInfo, previousMoreInfo);

		}
		public BusinessEntityHistoryStack GetLastBusinessEntityHistoryStack(Guid businessEntityDefinitionId, string businessEntityId, string fieldName)
		{
			IBusinessEntityHistoryStackDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityHistoryStackDataManager>();
			return dataManager.GetLastBusinessEntityHistoryStack(businessEntityDefinitionId, businessEntityId, fieldName);
		}
		public Vanrise.Entities.IDataRetrievalResult<BusinessEntityHistoryStackDetail> GetFilteredBusinessEntitiesHistoryStack(Vanrise.Entities.DataRetrievalInput<BusinessEntityHistoryStackQuery> input)
		{
			IBusinessEntityHistoryStackDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityHistoryStackDataManager>();
			IEnumerable<BusinessEntityHistoryStack> businessEntitiesStatusHistory = dataManager.GetFilteredBusinessEntitiesHistoryStack(input);

			Func<BusinessEntityHistoryStack, bool> filterExpression = (item) =>
			{
				return true;
			};
			return DataRetrievalManager.Instance.ProcessResult(input, businessEntitiesStatusHistory.ToBigResult(input, filterExpression, BusinessEntityHistoryStackDetailMapper));
		}
		public bool DeleteHistoryStack(long id)
		{
			IBusinessEntityHistoryStackDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityHistoryStackDataManager>();
			return	dataManager.DeleteBusinessEntityHistoryStack(id);
		}
		private BusinessEntityHistoryStackDetail BusinessEntityHistoryStackDetailMapper(BusinessEntityHistoryStack businessEntityStatusHistory)
		{
			var statusDefinitionManager = new StatusDefinitionManager();

			var businessEntityStatusHistoryDetail = new BusinessEntityHistoryStackDetail
			{
				BusinessEntityStatusHistoryId = businessEntityStatusHistory.BusinessEntityHistoryStackId,
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
