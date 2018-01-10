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

        public VRMailMessageTemplate GetMailMessageTemplate(Guid vrMailMessageTemplateId, bool isViewedFromUI)
        {
            Dictionary<Guid, VRMailMessageTemplate> cachedVRMailMessageTemplates = this.GetCachedVRMailMessageTemplates();
            VRMailMessageTemplate vrmailMessageTemplate = cachedVRMailMessageTemplates.GetRecord(vrMailMessageTemplateId);
            if (vrmailMessageTemplate != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(VRMailMessageTemplateLoggableEntity.Instance, vrmailMessageTemplate);
            return vrmailMessageTemplate;
        }

        public VRMailMessageTemplate GetMailMessageTemplate(Guid vrMailMessageTemplateId)
        {
            return GetMailMessageTemplate(vrMailMessageTemplateId, false);
        }

        public IDataRetrievalResult<VRMailMessageTemplateDetail> GetFilteredMailMessageTemplates(DataRetrievalInput<VRMailMessageTemplateQuery> input)
        {
            var allVRMailMessageTemplates = GetCachedVRMailMessageTemplates();
            Func<VRMailMessageTemplate, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(VRMailMessageTemplateLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRMailMessageTemplates.ToBigResult(input, filterExpression, VRMailMessageTemplateDetailMapper));
        }

        public string GetMAilMessageTemplateName(VRMailMessageTemplate vrMailMessageTemplateId)
        {
            return vrMailMessageTemplateId != null ? vrMailMessageTemplateId.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<VRMailMessageTemplateDetail> AddMailMessageTemplate(VRMailMessageTemplate vrMailMessageTemplateItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRMailMessageTemplateDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            if (!IsMailTemplateValid(vrMailMessageTemplateItem))
            {
                insertOperationOutput.ShowExactMessage = true;
                insertOperationOutput.Message = "Template Validation Error. Check Log";
                return insertOperationOutput;
            }

            IVRMailMessageTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTemplateDataManager>();

            vrMailMessageTemplateItem.VRMailMessageTemplateId = Guid.NewGuid();

            if (dataManager.Insert(vrMailMessageTemplateItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRMailMessageTemplateLoggableEntity.Instance, vrMailMessageTemplateItem);
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

            if (!IsMailTemplateValid(vrMailMessageTemplateItem))
            {
                updateOperationOutput.ShowExactMessage = true;
                updateOperationOutput.Message = "Template Validation Error. Check Log";
                return updateOperationOutput;
            }

            IVRMailMessageTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTemplateDataManager>();

            if (dataManager.Update(vrMailMessageTemplateItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRMailMessageTemplateLoggableEntity.Instance, vrMailMessageTemplateItem);
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

        #region Private Methods

        bool IsMailTemplateValid(VRMailMessageTemplate mailTemplate)
        {
            if (mailTemplate == null || mailTemplate.Settings == null)
                return false;

            if (!IsMailTemplateExpressionValid(mailTemplate.Settings.From))
                return false;

            if (!IsMailTemplateExpressionValid(mailTemplate.Settings.To))
                return false;

            if (!IsMailTemplateExpressionValid(mailTemplate.Settings.CC))
                return false;

            if (!IsMailTemplateExpressionValid(mailTemplate.Settings.BCC))
                return false;

            if (!IsMailTemplateExpressionValid(mailTemplate.Settings.Subject))
                return false;

            if (!IsMailTemplateExpressionValid(mailTemplate.Settings.Body))
                return false;

            return true;
        }

        bool IsMailTemplateExpressionValid(VRExpression expression)
        {
            if (expression != null && !string.IsNullOrEmpty(expression.ExpressionString))
            {
                try
                {
                    string templateKeyLocal = string.Format("TemplateKey_{0}", Guid.NewGuid());
                    var key = new RazorEngine.Templating.NameOnlyTemplateKey(templateKeyLocal, RazorEngine.Templating.ResolveType.Global, null);
                    RazorEngine.Engine.Razor.AddTemplate(key, new RazorEngine.Templating.LoadedTemplateSource(expression.ExpressionString));
                    RazorEngine.Engine.Razor.Compile(key, typeof(VRMailContext));
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().WriteError(ex.Message.Replace('{','[').Replace('}',']'));
                    return false;
                }
            }

            return true;
        }

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

        private class VRMailMessageTemplateLoggableEntity : VRLoggableEntityBase
        {
            public static VRMailMessageTemplateLoggableEntity Instance = new VRMailMessageTemplateLoggableEntity();

            private VRMailMessageTemplateLoggableEntity()
            {

            }

            static VRMailMessageTemplateManager s_vrmailMessageTemplateManager = new VRMailMessageTemplateManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_VRMailMessageTemplate"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Mail Message Template"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_VRMailMessageTemplate_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRMailMessageTemplate vrmailMessageTemplate = context.Object.CastWithValidate<VRMailMessageTemplate>("context.Object");
                return vrmailMessageTemplate.VRMailMessageTemplateId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRMailMessageTemplate vrmailMessageTemplate = context.Object.CastWithValidate<VRMailMessageTemplate>("context.Object");
                return s_vrmailMessageTemplateManager.GetMAilMessageTemplateName(vrmailMessageTemplate);
            }
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

                VRMailMessageTypeName = vrMailMessageTypeName
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