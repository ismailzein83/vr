using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRMailMessageTemplateManager
    {
        #region Public Methods
        public VRMailMessageTemplate GetMailMessageTemplate(Guid vrMailMessageTemplateId)
        {
            Dictionary<Guid, VRMailMessageTemplate> cachedVRMailMessageTemplates = this.GetCachedVRMailMessageTemplates();
            return cachedVRMailMessageTemplates.GetRecord(vrMailMessageTemplateId);
        }

        public IDataRetrievalResult<VRMailMessageTemplateDetail> GetFilteredMailMessageTemplates(DataRetrievalInput<VRMailMessageTemplateQuery> input)
        {
            var allVRMailMessageTemplates = GetCachedVRMailMessageTemplates();
            Func<VRMailMessageTemplate, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRMailMessageTemplates.ToBigResult(input, filterExpression, VRMailMessageTemplateDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<VRMailMessageTemplateDetail> AddMailMessageTemplate(VRMailMessageTemplate vrMailMessageTemplateItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRMailMessageTemplateDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRMailMessageTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTemplateDataManager>();

            vrMailMessageTemplateItem.VRMailMessageTemplateId = Guid.NewGuid();

            if (dataManager.Insert(vrMailMessageTemplateItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRMailMessageTemplateDetailMapper(vrMailMessageTemplateItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRMailMessageTemplateDetail> UpdateMailMessageTemplate(VRMailMessageTemplate vrMailMessageTemplateItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRMailMessageTemplateDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRMailMessageTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTemplateDataManager>();

            if (dataManager.Update(vrMailMessageTemplateItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRMailMessageTemplateDetailMapper(this.GetMailMessageTemplate(vrMailMessageTemplateItem.VRMailMessageTemplateId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<VRMailMessageTemplateInfo> GetMailMessageTemplatesInfo(VRMailMessageTemplateFilter filter)
        {
            Func<VRMailMessageTemplate, bool> filterExpression = (item) =>
            {
                if (filter == null)
                    return true;

                if (filter.VRMailMessageTypeId != item.VRMailMessageTypeId)
                    return false;

                return true;
            };



            return this.GetCachedVRMailMessageTemplates().MapRecords(VRMailMessageTemplateInfoMapper, filterExpression).OrderBy(x => x.Name);
        }
        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRMailMessageTemplateDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreMailMessageTemplateUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, VRMailMessageTemplate> GetCachedVRMailMessageTemplates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRMailMessageTemplates",
               () =>
               {
                   IVRMailMessageTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTemplateDataManager>();
                   return dataManager.GetMailMessageTemplates().ToDictionary(x => x.VRMailMessageTemplateId, x => x);
               });
        }

        #endregion

        #region Mappers
        public VRMailMessageTemplateDetail VRMailMessageTemplateDetailMapper(VRMailMessageTemplate vrMailMessageTemplate)
        {
            VRMailMessageTypeManager vrMailMessageTypeManager = new VRMailMessageTypeManager();
            VRMailMessageType vrMailMessageType = vrMailMessageTypeManager.GetMailMessageType(vrMailMessageTemplate.VRMailMessageTypeId);
            string vrMailMessageTypeName = vrMailMessageType != null ? vrMailMessageType.Name : string.Empty;

            VRMailMessageTemplateDetail vrMailMessageTemplateDetail = new VRMailMessageTemplateDetail()
            {
                Entity = vrMailMessageTemplate,

                VRMailMessageTypeName =  vrMailMessageTypeName
            };
            return vrMailMessageTemplateDetail;
        }

        public VRMailMessageTemplateInfo VRMailMessageTemplateInfoMapper(VRMailMessageTemplate vrMailMessageTemplate)
        {
            VRMailMessageTemplateInfo vrMailMessageTemplateInfo = new VRMailMessageTemplateInfo()
            {
                VRMailMessageTemplateId = vrMailMessageTemplate.VRMailMessageTemplateId,
                Name = vrMailMessageTemplate.Name
            };
            return vrMailMessageTemplateInfo;
        }
        #endregion
    }
}
