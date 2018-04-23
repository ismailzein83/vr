using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Entities.SMS;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class SMSMessageTemplateManager
    {
        #region Public Methods
        public SMSMessageTemplate GetSMSMessageTemplate(Guid smsMessageTemplateId, bool isViewedFromUI)
        {
            var cachedSMSMessageTemplates = this.GetCachedSMSMessageTemplates();
           
            SMSMessageTemplate smsMessageTemplate = cachedSMSMessageTemplates.GetRecord(smsMessageTemplateId);
           
            if (smsMessageTemplate != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(SMSMessageTemplateLoggableEntity.Instance, smsMessageTemplate);
        
            return smsMessageTemplate;
        }

        public SMSMessageTemplate GetSMSMessageTemplate(Guid smsMessageTemplateId)
        {
            return GetSMSMessageTemplate(smsMessageTemplateId, false);
        }

        public IDataRetrievalResult<SMSMessageTemplateDetail> GetFilteredSMSMessageTemplates(DataRetrievalInput<SMSMessageTemplateQuery> input)
        {
            var allSMSMessageTemplates = GetCachedSMSMessageTemplates();
            Func<SMSMessageTemplate, bool> filterExpression = (x) =>{
                if(input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };

            VRActionLogger.Current.LogGetFilteredAction(SMSMessageTemplateLoggableEntity.Instance, input);
        
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSMSMessageTemplates.ToBigResult(input, filterExpression, SMSMessageTemplateDetailMapper));
        }

        public IEnumerable<SMSMessageTemplateInfo> GetSMSMessageTemplatesInfo(SMSMessageTemplateFilter filter)
        {
            Func<SMSMessageTemplate, bool> filterExpression = null;
            filterExpression = (item) =>
            {
                if (filter == null)
                    return true;

                if (filter.SMSMessageTypeId != item.SMSMessageTypeId)
                    return false;

                return true;
            };

            return this.GetCachedSMSMessageTemplates().MapRecords(SMSMessageTemplateInfoMapper, filterExpression).OrderBy(x => x.Name);
        }
        public string GetSMSMessageTemplateName(SMSMessageTemplate smsMessageTemplateId)
        {
            return smsMessageTemplateId != null ? smsMessageTemplateId.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<SMSMessageTemplateDetail> AddSMSMessageTemplate(SMSMessageTemplate smsMessageTemplateItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SMSMessageTemplateDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            if (!IsSMSTemplateValid(smsMessageTemplateItem))
            {
                insertOperationOutput.ShowExactMessage = true;
                insertOperationOutput.Message = "Template Validation Error. Check Log";
                return insertOperationOutput;
            }

            ISMSMessageTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ISMSMessageTemplateDataManager>();

            smsMessageTemplateItem.SMSMessageTemplateId = Guid.NewGuid();

            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            smsMessageTemplateItem.CreatedBy = loggedInUserId;
            smsMessageTemplateItem.LastModifiedBy = loggedInUserId;
            
            if (dataManager.Insert(smsMessageTemplateItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(SMSMessageTemplateLoggableEntity.Instance, smsMessageTemplateItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SMSMessageTemplateDetailMapper(smsMessageTemplateItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SMSMessageTemplateDetail> UpdateSMSMessageTemplate(SMSMessageTemplate smsMessageTemplateItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SMSMessageTemplateDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (!IsSMSTemplateValid(smsMessageTemplateItem))
            {
                updateOperationOutput.ShowExactMessage = true;
                updateOperationOutput.Message = "Template Validation Error. Check Log";
                return updateOperationOutput;
            }

            ISMSMessageTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ISMSMessageTemplateDataManager>();

            smsMessageTemplateItem.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();

            if (dataManager.Update(smsMessageTemplateItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(SMSMessageTemplateLoggableEntity.Instance, smsMessageTemplateItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SMSMessageTemplateDetailMapper(this.GetSMSMessageTemplate(smsMessageTemplateItem.SMSMessageTemplateId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        bool IsSMSTemplateValid(SMSMessageTemplate smsTemplate)
        {
            if (smsTemplate == null || smsTemplate.Settings == null)
                return false;

            if (!IsSMSTemplateExpressionValid(smsTemplate.Settings.MobileNumber))
                return false;

            if (!IsSMSTemplateExpressionValid(smsTemplate.Settings.Message))
                return false;
            
            return true;
        }

        bool IsSMSTemplateExpressionValid(VRExpression expression)
        {
            if (expression != null && !string.IsNullOrEmpty(expression.ExpressionString))
            {
                try
                {
                    string templateKeyLocal = string.Format("TemplateKey_{0}", Guid.NewGuid());
                    var key = new RazorEngine.Templating.NameOnlyTemplateKey(templateKeyLocal, RazorEngine.Templating.ResolveType.Global, null);
                    RazorEngine.Engine.Razor.AddTemplate(key, new RazorEngine.Templating.LoadedTemplateSource(expression.ExpressionString));
                    RazorEngine.Engine.Razor.Compile(key, typeof(VRSMSContext));
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetLogger().WriteError(ex.Message.Replace('{', '[').Replace('}', ']'));
                    return false;
                }
            }

            return true;
        }

         Dictionary<Guid, SMSMessageTemplate> GetCachedSMSMessageTemplates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSMSMessageTemplates",
               () =>
               {
                   ISMSMessageTemplateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ISMSMessageTemplateDataManager>();
                   return dataManager.GetSMSMessageTemplates().ToDictionary(x => x.SMSMessageTemplateId, x => x);
               });
        }

        #endregion

        #region Private Classes
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISMSMessageTemplateDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ISMSMessageTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSMSMessageTemplateUpdated(ref _updateHandle);
            }
        }

        private class SMSMessageTemplateLoggableEntity : VRLoggableEntityBase
        {
            public static SMSMessageTemplateLoggableEntity Instance = new SMSMessageTemplateLoggableEntity();

            private SMSMessageTemplateLoggableEntity()
            {

            }

            static SMSMessageTemplateManager s_smsMessageTemplateManager = new SMSMessageTemplateManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_SMSMessageTemplate"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "SMS Message Template"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_SMSMessageTemplate_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SMSMessageTemplate smsMessageTemplate = context.Object.CastWithValidate<SMSMessageTemplate>("context.Object");
                return smsMessageTemplate.SMSMessageTemplateId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SMSMessageTemplate smsMessageTemplate = context.Object.CastWithValidate<SMSMessageTemplate>("context.Object");
                return s_smsMessageTemplateManager.GetSMSMessageTemplateName(smsMessageTemplate);
            }
        }

        #endregion

        #region Mappers
       public SMSMessageTemplateDetail SMSMessageTemplateDetailMapper(SMSMessageTemplate smsMessageTemplate)
        {
            SMSMessageTemplateDetail smsMessageTemplateDetail = new SMSMessageTemplateDetail()
            {
                Entity = smsMessageTemplate,
            };
            return smsMessageTemplateDetail;
        }

       public SMSMessageTemplateInfo SMSMessageTemplateInfoMapper(SMSMessageTemplate smsMessageTemplate)
       {
           SMSMessageTemplateInfo smsMessageTemplateInfo = new SMSMessageTemplateInfo()
           {
               SMSMessageTemplateId = smsMessageTemplate.SMSMessageTemplateId,
               Name = smsMessageTemplate.Name
           };
           return smsMessageTemplateInfo;
       }

        #endregion

    }
    public class VRSMSContext
    {
        VRObjectEvaluator vrObjectEvaluator;

        public VRSMSContext(SMSMessageTypeSettings smsMessageTypeSettings, Dictionary<string, dynamic> objects)
        {
            vrObjectEvaluator = new VRObjectEvaluator(smsMessageTypeSettings.Objects, objects);
        }

        public dynamic GetVal(string objectName, string propertyName)
        {
            return vrObjectEvaluator.GetPropertyValue(objectName, propertyName);
        }
    }
}
