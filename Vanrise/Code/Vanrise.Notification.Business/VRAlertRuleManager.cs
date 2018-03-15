using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Notification.Data;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleManager
    {
        #region Ctor/Fields

        static VRAlertRuleTypeManager _alertTypeManager;

        public VRAlertRuleManager()
        {
            _alertTypeManager = new VRAlertRuleTypeManager();
        }

        #endregion

        #region Public Methods

        public List<VRAlertRule> GetActiveRules(Guid ruleTypeId)
        {
            return GetCachedRulesByType().GetRecord(ruleTypeId);
        }

        public VRAlertRule GetVRAlertRule(long vrAlertRuleId)
        {

            return GetVRAlertRule(vrAlertRuleId, false);
        }

        public VRAlertRule GetVRAlertRule(long vrAlertRuleId, bool isViewedFromUI)
        {
            Dictionary<long, VRAlertRule> cachedVRAlertRules = this.GetCachedVRAlertRules();
            var vrAlertRule = cachedVRAlertRules.GetRecord(vrAlertRuleId);
            if (vrAlertRule != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(new VRAlertRuleLoggableEntity(vrAlertRule.RuleTypeId), vrAlertRule);
            return vrAlertRule;
        }

        public string GetAlertRuleName(long vrAlertRuleId)
        {
            var vrAlertRule = GetVRAlertRule(vrAlertRuleId);
            if (vrAlertRule == null)
                return null;
            return vrAlertRule.Name;
        }

        public IDataRetrievalResult<VRAlertRuleDetail> GetFilteredVRAlertRules(DataRetrievalInput<VRAlertRuleQuery> input)
        {
            var allVRAlertRules = this.GetCachedVRAlertRules();
            var allowedAlertRuleTypeIds = _alertTypeManager.GetAllowedRuleTypeIds();
            Func<VRAlertRule, bool> filterExpression = (x) =>
            {
                if (!allowedAlertRuleTypeIds.Contains(x.RuleTypeId))
                    return false;
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.RuleTypeIds != null && !input.Query.RuleTypeIds.Contains(x.RuleTypeId))
                    return false;
                return true;
            };

            ResultProcessingHandler<VRAlertRuleDetail> handler = new ResultProcessingHandler<VRAlertRuleDetail>()
            {
                ExportExcelHandler = new VRAlertRuleExcelExportHandler()
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRAlertRules.ToBigResult(input, filterExpression, VRAlertRuleDetailMapper), handler);
        }
         
        public Vanrise.Entities.InsertOperationOutput<VRAlertRuleDetail> AddVRAlertRule(VRAlertRule vrAlertRuleItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRAlertRuleDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long vrAlertRuleId = -1;

            //Adding UserId to VRAlertRule
            vrAlertRuleItem.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            IVRAlertRuleDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleDataManager>();

            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            vrAlertRuleItem.CreatedBy = loggedInUserId;
            vrAlertRuleItem.LastModifiedBy = loggedInUserId;

            if (dataManager.Insert(vrAlertRuleItem, out vrAlertRuleId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                vrAlertRuleItem.VRAlertRuleId = vrAlertRuleId;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                VRActionLogger.Current.TrackAndLogObjectAdded(new VRAlertRuleLoggableEntity(vrAlertRuleItem.RuleTypeId), vrAlertRuleItem);
                insertOperationOutput.InsertedObject = VRAlertRuleDetailMapper(vrAlertRuleItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRAlertRuleDetail> UpdateVRAlertRule(VRAlertRule vrAlertRuleItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRAlertRuleDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRAlertRuleDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleDataManager>();
            
            vrAlertRuleItem.LastModifiedBy = SecurityContext.Current.GetLoggedInUserId();

            if (dataManager.Update(vrAlertRuleItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(new VRAlertRuleLoggableEntity(vrAlertRuleItem.RuleTypeId), vrAlertRuleItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRAlertRuleDetailMapper(this.GetVRAlertRule(vrAlertRuleItem.VRAlertRuleId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        //public IEnumerable<VRAlertRuleInfo> GetVRAlertRulesInfo(VRAlertRuleFilter filter)
        //{
        //    Func<VRAlertRule, bool> filterExpression = null;

        //    return this.GetCachedVRAlertRules().MapRecords(VRAlertRuleInfoMapper, filterExpression).OrderBy(x => x.Name);
        //}
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRAlertRuleDataManager _dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRAlertRuleUpdated(ref _updateHandle);
            }
        }

        public class VRAlertRuleLoggableEntity : VRLoggableEntityBase
        {

            Guid _ruleTypeId;
            static VRAlertRuleTypeManager _vralertRuleTypeManager = new VRAlertRuleTypeManager();
            static VRAlertRuleManager s_vrruleAlertManager = new VRAlertRuleManager();

            public VRAlertRuleLoggableEntity(Guid ruleTypeId)
            {
                _ruleTypeId = ruleTypeId;

            }

            public override string EntityUniqueName
            {
                get { return String.Format("VR_Notification_AlertRule_{0}", _ruleTypeId); }
            }

            public override string EntityDisplayName
            {
                get
                {
                    return String.Format(_vralertRuleTypeManager.GetVRAlertRuleTypeName(_ruleTypeId), "_AlertRules");

                }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Notification_AlertRule_ViewHistoryItem"; }
            }


            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRAlertRule vrAlertRule = context.Object.CastWithValidate<VRAlertRule>("context.Object");
                return vrAlertRule.VRAlertRuleId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRAlertRule vrAlertRule = context.Object.CastWithValidate<VRAlertRule>("context.Object");
                return s_vrruleAlertManager.GetAlertRuleName(vrAlertRule.VRAlertRuleId);
            }

            public override string ModuleName
            {
                get { return "Notification"; }
            }
        }

        private class VRAlertRuleExcelExportHandler : ExcelExportHandler<VRAlertRuleDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<VRAlertRuleDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Action Rules",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Id" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name", Width = 35});
                
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.VRAlertRuleId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<Guid, List<VRAlertRule>> GetCachedRulesByType()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRAlertRulesByTypeId",
               () =>
               {
                   return GetCachedVRAlertRules().Values.GroupBy(s => s.RuleTypeId).ToDictionary(x => x.Key, v => v.ToList());
               });
        }

        public Dictionary<long, VRAlertRule> GetCachedVRAlertRules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRAlertRules",
               () =>
               {
                   IVRAlertRuleDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleDataManager>();
                   return dataManager.GetVRAlertRules().ToDictionary(x => x.VRAlertRuleId, x => x);
               });
        }

        #endregion

        #region Mappers

        public VRAlertRuleDetail VRAlertRuleDetailMapper(VRAlertRule vrAlertRule)
        {
            VRAlertRuleDetail vrAlertRuleDetail = new VRAlertRuleDetail()
            {
                Entity = vrAlertRule,
                RuleTypeName = new VRAlertRuleTypeManager().GetVRAlertRuleTypeName(vrAlertRule.RuleTypeId)
            };
            vrAlertRuleDetail.AllowEdit = _alertTypeManager.DoesUserHaveEditAccess(vrAlertRule.RuleTypeId);
            return vrAlertRuleDetail;
        }

        //public VRAlertRuleInfo VRAlertRuleInfoMapper(VRAlertRule vrAlertRule)
        //{
        //    VRAlertRuleInfo vrAlertRuleInfo = new VRAlertRuleInfo()
        //    {
        //        VRAlertRuleId = vrAlertRule.VRAlertRuleId,
        //        Name = vrAlertRule.Name
        //    };
        //    return vrAlertRuleInfo;
        //}

        #endregion
    }
}
