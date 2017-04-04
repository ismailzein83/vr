using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class DIDManager : IBusinessEntityManager
    {
        #region ctor/Local Variables

        Guid _accountDIDRelationDefinitionId;

        public DIDManager()
        {
            _accountDIDRelationDefinitionId = new ConfigManager().GetAccountDIDRelationDefinitionId();
        }

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<DIDDetail> GetFilteredDIDs(Vanrise.Entities.DataRetrievalInput<DIDQuery> input)
        {
            var allDIDs = GetCachedDIDs();
            Func<DID, bool> filterExpression = (did) => (input.Query.Number == null || did.Number.ToLower().Contains(input.Query.Number.ToLower()));

            ResultProcessingHandler<DIDDetail> handler = new ResultProcessingHandler<DIDDetail>()
            {
                ExportExcelHandler = new DIDExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(DIDLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allDIDs.ToBigResult(input, filterExpression, DIDDetailMapper), handler);
        }

        public DID GetDID(int didId, bool isViewedFromUI)
        {
            var dids = GetCachedDIDs();
            var did = dids.GetRecord(didId);
           if (did != null && isViewedFromUI)
               VRActionLogger.Current.LogObjectViewed(DIDLoggableEntity.Instance, did);
           return did;
        }
        public DID GetDID(int didId)
        {
            return  GetDID(didId,false);
        }
       
        public string GetDIDNumber(int didId)
        {
            var DIDs = GetCachedDIDs();
            DID did = DIDs.GetRecord(didId);
            if (did == null)
                throw new NullReferenceException(string.Format("DID ID {0}", didId));

            return did.Number;
        }

        public DID GetDIDByNumber(string number)
        {
            var dids = GetCachedDIDsGroupByNumber();

            if (dids == null)
                return null;

            return dids.GetRecord(number);
        }

        public InsertOperationOutput<DIDDetail> AddDID(DID did)
        {
            InsertOperationOutput<DIDDetail> insertOperationOutput = new InsertOperationOutput<DIDDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int didId = -1;

            IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
            bool insertActionSucc = dataManager.Insert(did, out didId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                did.DIDId = didId;
                VRActionLogger.Current.TrackAndLogObjectAdded(DIDLoggableEntity.Instance, did);
                insertOperationOutput.InsertedObject = DIDDetailMapper(did);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<DIDDetail> UpdateDID(DID did)
        {
            IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();

            bool updateActionSucc = dataManager.Update(did);
            UpdateOperationOutput<DIDDetail> updateOperationOutput = new UpdateOperationOutput<DIDDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(DIDLoggableEntity.Instance, did);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DIDDetailMapper(did);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<DIDInfo> GetDIDsInfo(DIDFilter filter)
        {
            Func<DID, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (did) =>
                {
                    if (filter.Filters != null && !CheckIfFilterIsMatch(did, filter.Filters))
                        return false;

                    return true;
                };
            }

            return GetCachedDIDs().MapRecords(DIDInfoMapper, filterExpression);
        }

        public Guid GetAccountDIDRelationDefinitionId()
        {
            return _accountDIDRelationDefinitionId;
        }

        public bool IsDIDAssignedToParentWithoutEED(Guid accountDIDRelationDefinitionId, string childId)
        {
            return new BEParentChildRelationManager().IsChildAssignedToParentWithoutEED(accountDIDRelationDefinitionId, childId);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, DID> GetCachedDIDsGroupByNumber()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDIDsGroupByNumber",
               () =>
               {
                   var dids = GetCachedDIDs();
                   return dids.ToDictionary(itm => itm.Value.Number, itm => itm.Value);
               });
        }

        private Dictionary<int, DID> GetCachedDIDs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllDIDs",
               () =>
               {
                   IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
                   IEnumerable<DID> DIDs = dataManager.GetAllDIDs();
                   return DIDs.ToDictionary(x => x.DIDId, x => x);
               });
        }

        private bool CheckIfFilterIsMatch(DID did, List<IDIDFilter> filters)
        {
            DIDFilterContext context = new DIDFilterContext { DID = did };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region Private Classes
        private class DIDExcelExportHandler : ExcelExportHandler<DIDDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DIDDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "DIDs",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Account" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Number of channels" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Number" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "International" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null && record.Entity.Settings != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.AccountName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.NumberOfChannels });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Number });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.IsInternational });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDIDDataManager _dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDIDsUpdated(ref _updateHandle);
            }
        }


        private class DIDLoggableEntity : VRLoggableEntityBase
        {
            public static DIDLoggableEntity Instance = new DIDLoggableEntity();
            static DIDManager s_didManager = new DIDManager();

            public override string EntityUniqueName
            {
                get { return "Retail_BusinessEntity_DID"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "DID"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "Retail_BusinessEntity_DID_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                DID did = context.Object.CastWithValidate<DID>("context.Object");
                return did.DIDId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DID did = context.Object.CastWithValidate<DID>("context.Object");
                return (s_didManager.GetDIDNumber(did.DIDId) != null) ? s_didManager.GetDIDNumber(did.DIDId) : did.DIDId.ToString();
            }
        }

        #endregion

        #region  Mappers

        private DIDDetail DIDDetailMapper(DID did)
        {
            DIDDetail didDetail = new DIDDetail();
            didDetail.Entity = did;

            BEParentChildRelation beParentChildRelation = new BEParentChildRelationManager().GetParent(_accountDIDRelationDefinitionId, did.DIDId.ToString(), DateTime.Now);
            if (beParentChildRelation != null)
            {
                BEParentChildRelationDefinition beParentChildRelationDefinition = new BEParentChildRelationDefinitionManager().GetBEParentChildRelationDefinition(_accountDIDRelationDefinitionId);
                didDetail.AccountName = new BusinessEntityManager().GetEntityDescription(beParentChildRelationDefinition.Settings.ParentBEDefinitionId, long.Parse(beParentChildRelation.ParentBEId));
            }
            return didDetail;
        }

        private DIDInfo DIDInfoMapper(DID did)
        {
            return new DIDInfo()
            {
                DIDId = did.DIDId,
                Number = did.Number,
            };
        }

        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetCachedDIDs().Select(itm => itm as dynamic).ToList();
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetDID(context.EntityId);
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetDIDNumber(Int32.Parse(context.EntityId.ToString()));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var did = context.Entity as DID;
            return did.DIDId;
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
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
