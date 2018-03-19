using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SellingNumberPlanManager : IBusinessEntityManager , ISellingNumberPlanManager
    {
        #region Public Methods
        public IEnumerable<SellingNumberPlan> GetAllSellingNumberPlans()
        {
            var allSellingNumberPlans = GetCachedSellingNumberPlans();
            if (allSellingNumberPlans == null)
                return null;

            return allSellingNumberPlans.Values;
        }

        public IEnumerable<SellingNumberPlanInfo> GetSellingNumberPlans()
        {
            return GetCachedSellingNumberPlans().Values.MapRecords(SellingNumberPlanInfoMapper).OrderBy(x => x.Name);
        }

        public SellingNumberPlan GetSellingNumberPlanHistoryDetailbyHistoryId(int sellingNumberPlanHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var sellingNumberPlan = s_vrObjectTrackingManager.GetObjectDetailById(sellingNumberPlanHistoryId);
            return sellingNumberPlan.CastWithValidate<SellingNumberPlan>("SellingNumberPlan : historyId ", sellingNumberPlanHistoryId);
        }

        public SellingNumberPlan GetSellingNumberPlan(int numberPlanId, bool isViewedFromUI)
        {
            var sellingNumberPlan= GetCachedSellingNumberPlans().GetRecord(numberPlanId);

            if (sellingNumberPlan != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(SellingNumberPlanLoggableEntity.Instance, sellingNumberPlan);
            return sellingNumberPlan;
        }
        public SellingNumberPlan GetSellingNumberPlan(int numberPlanId)
        {
            return  GetSellingNumberPlan(numberPlanId, false);
        }

        public IDataRetrievalResult<SellingNumberPlanDetail> GetFilteredSellingNumberPlans(DataRetrievalInput<SellingNumberPlanQuery> input)
        {
            var allSellingNumberPlans = GetCachedSellingNumberPlans();
            Func<SellingNumberPlan, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));

            ResultProcessingHandler<SellingNumberPlanDetail> handler = new ResultProcessingHandler<SellingNumberPlanDetail>()
            {
                ExportExcelHandler = new SellingNumberPlanExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(SellingNumberPlanLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSellingNumberPlans.ToBigResult(input, filterExpression, SellingNumberPlanDetailMapper), handler);

        }

        public InsertOperationOutput<SellingNumberPlanDetail> AddSellingNumberPlan(SellingNumberPlan sellingNumberPlan)
        {
            ValidateSellingNumberPlanToAdd(sellingNumberPlan);

            InsertOperationOutput<SellingNumberPlanDetail> insertOperationOutput = new InsertOperationOutput<SellingNumberPlanDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int sellingNumberPlanId = -1;

            ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();

            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            sellingNumberPlan.CreatedBy = loggedInUserId;
            sellingNumberPlan.LastModifiedBy = loggedInUserId;

            bool insertActionSucc = dataManager.Insert(sellingNumberPlan, out sellingNumberPlanId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                sellingNumberPlan.SellingNumberPlanId = sellingNumberPlanId;
                VRActionLogger.Current.TrackAndLogObjectAdded(SellingNumberPlanLoggableEntity.Instance, sellingNumberPlan);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SellingNumberPlanDetailMapper(sellingNumberPlan);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<SellingNumberPlanDetail> UpdateSellingNumberPlan(SellingNumberPlanToEdit sellingNumberPlanToEdit)
        {
            ValidateSellingNumberPlanToEdit(sellingNumberPlanToEdit);

            ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();

            sellingNumberPlanToEdit.LastModifiedBy = SecurityContext.Current.GetLoggedInUserId();

            bool updateActionSucc = dataManager.Update(sellingNumberPlanToEdit);
            UpdateOperationOutput<SellingNumberPlanDetail> updateOperationOutput = new UpdateOperationOutput<SellingNumberPlanDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var sellingNumberPlan = GetSellingNumberPlan(sellingNumberPlanToEdit.SellingNumberPlanId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(SellingNumberPlanLoggableEntity.Instance, sellingNumberPlan);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SellingNumberPlanDetailMapper(this.GetSellingNumberPlan(sellingNumberPlanToEdit.SellingNumberPlanId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public string GetDescription(IEnumerable<int> sellingNumberPlanIds)
        {
            List<string> sellingNumberPlanNames = new List<string>();
            foreach (int sellingNumberPlanId in sellingNumberPlanIds)
            {
                SellingNumberPlan sellingNumberPlan = GetSellingNumberPlan(sellingNumberPlanId);
                sellingNumberPlanNames.Add(sellingNumberPlan.Name);
            }

            if (sellingNumberPlanNames != null)
                return string.Join(", ", sellingNumberPlanNames.Select(x => x));

            return string.Empty;
        }
       
        public SellingNumberPlan GetMasterSellingNumberPlan()
        {
            var allNumberPlans = GetCachedSellingNumberPlans();
            if (allNumberPlans != null)
                return allNumberPlans.Values.FirstOrDefault();
            else
                return null;
        }

        #endregion

        #region ISellingNumberPlanManager Memebers
       
        public string GetSellingNumberPlanName(int sellingNumberPlanId)
        {
            SellingNumberPlan sellingNumberPlan = GetSellingNumberPlan(sellingNumberPlanId);

            if (sellingNumberPlan != null)
                return sellingNumberPlan.Name;

            return null;
        }
       
        #endregion

        #region Validation Methods

        void ValidateSellingNumberPlanToAdd(SellingNumberPlan sellingNumberPlan)
        {
            ValidateSellingNumberPlan(sellingNumberPlan.Name);
        }

        void ValidateSellingNumberPlanToEdit(SellingNumberPlanToEdit sellingNumberPlan)
        {
            ValidateSellingNumberPlan(sellingNumberPlan.Name);
        }

        void ValidateSellingNumberPlan(string snpName)
        {
            if (String.IsNullOrWhiteSpace(snpName))
                throw new MissingArgumentValidationException("SellingNumberPlan.Name");
        }

        #endregion

        #region Private Classes

        private class SellingNumberPlanExcelExportHandler : ExcelExportHandler<SellingNumberPlanDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SellingNumberPlanDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Selling Number Plans",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });

                sheet.Rows = new List<ExportExcelRow>();

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.SellingNumberPlanId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISellingNumberPlanDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSellingNumberPlansUpdated(ref _updateHandle);
            }
        }


        private class SellingNumberPlanLoggableEntity : VRLoggableEntityBase
        {
            public static SellingNumberPlanLoggableEntity Instance = new SellingNumberPlanLoggableEntity();

            private SellingNumberPlanLoggableEntity()
            {

            }

            static SellingNumberPlanManager s_sellingNumberPlanManager = new SellingNumberPlanManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SellingNumberPlan"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Selling Number Plan"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SellingNumberPlan_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SellingNumberPlan sellingNumberPlan = context.Object.CastWithValidate<SellingNumberPlan>("context.Object");
                return sellingNumberPlan.SellingNumberPlanId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SellingNumberPlan sellingNumberPlan = context.Object.CastWithValidate<SellingNumberPlan>("context.Object");
                return s_sellingNumberPlanManager.GetSellingNumberPlanName(sellingNumberPlan.SellingNumberPlanId);
            }
        }
        #endregion

        #region Private Methods

        Dictionary<int, SellingNumberPlan> GetCachedSellingNumberPlans()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSellingNumberPlans",
               () =>
               {
                   ISellingNumberPlanDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingNumberPlanDataManager>();
                   return dataManager.GetSellingNumberPlans().ToDictionary(x => x.SellingNumberPlanId, x => x);
               });

        }

        public SellingNumberPlanDetail SellingNumberPlanDetailMapper(SellingNumberPlan sellingNumberPlan)
        {
            SellingNumberPlanDetail sellingNumberPlanDetail = new SellingNumberPlanDetail()
            {
                Entity = sellingNumberPlan
            };
            return sellingNumberPlanDetail;
        }

        private SellingNumberPlanInfo SellingNumberPlanInfoMapper(SellingNumberPlan sellingNumberPlan)
        {
            return new SellingNumberPlanInfo
            {
                Name = sellingNumberPlan.Name,
                SellingNumberPlanId = sellingNumberPlan.SellingNumberPlanId
            };
        }

        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allSellingNumberPlans = GetCachedSellingNumberPlans();
            if (allSellingNumberPlans == null)
                return null;
            else
                return allSellingNumberPlans.Values.Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetSellingNumberPlan(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetSellingNumberPlanName(Convert.ToInt32(context.EntityId));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var sellingNumberPlan = context.Entity as SellingNumberPlan;
            return sellingNumberPlan.SellingNumberPlanId;
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
