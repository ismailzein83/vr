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
using Vanrise.Common;

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
            Func<DID, bool> filterExpression = (did) => (input.Query.Number == null || did.ContainNumber(input.Query.Number));

            ResultProcessingHandler<DIDDetail> handler = new ResultProcessingHandler<DIDDetail>()
            {
                ExportExcelHandler = new DIDExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(DIDLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allDIDs.ToBigResult(input, filterExpression, DIDDetailMapper), handler);
        }

        public List<DID> GetDIDsByParentId(string parentId, DateTime effectiveOn)
        {
            BEParentChildRelationManager beParentChildRelationManager = new Vanrise.GenericData.Business.BEParentChildRelationManager();
            var relationItems = beParentChildRelationManager.GetChildren(_accountDIDRelationDefinitionId, parentId, effectiveOn);
            List<DID> dids = null;
            if (relationItems != null)
            {
                dids = new List<DID>();
                foreach (var item in relationItems)
                {
                    dids.Add(GetDID(Convert.ToInt32(item.ChildBEId)));
                }
            }
            return dids;
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
            return GetDID(didId, false);
        }

        public string GetDIDNumberDescription(int didId)
        {
            var DIDs = GetCachedDIDs();
            DID did = DIDs.GetRecord(didId);
            if (did == null)
                throw new NullReferenceException(string.Format("DID ID {0}", didId));

            return did.Description;
        }

        public DID GetDIDByNumber(string number)
        {
            var dids = GetCachedDIDsGroupByNumber();

            if (dids == null)
                return null;

            return dids.GetRecord(number);
        }

        public DID GetDIDBySourceId(string sourceId)
        {
            var dids = GetCachedDIDsGroupBySourceId();

            if (dids == null)
                return null;

            return dids.GetRecord(sourceId);
        }

        public bool TryAddDID(DID did, out int didId)
        {
            didId = 0;
            var cachedDIDs = GetCachedDIDsGroupByNumber();
            bool alreadyExist = false;
            switch (did.DIDNumberType)
            {
                case DIDNumberType.Number:
                    foreach (string number in did.Settings.Numbers)
                    {
                        alreadyExist = cachedDIDs.ContainsKey(number);
                        if (alreadyExist)
                            break;
                    }
                    break;
                case DIDNumberType.Range:
                    foreach (DIDRange range in did.Settings.Ranges)
                    {
                        long from = range.From.TryParseWithValidate<long>(long.TryParse);
                        long to = range.To.TryParseWithValidate<long>(long.TryParse);

                        for (long index = from; index <= to; index++)
                        {
                            alreadyExist = cachedDIDs.ContainsKey(index.ToString());
                            if (alreadyExist)
                                break;
                        }

                        if (alreadyExist)
                            break;
                    }
                    break;
                default: throw new Exception("Invalid Type for DID.");
            }
            if (!alreadyExist)
            {
                IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
                return dataManager.Insert(did, out didId);
            }
            return false;
        }
        public InsertOperationOutput<DIDDetail> AddDID(DID did)
        {
            InsertOperationOutput<DIDDetail> insertOperationOutput = new InsertOperationOutput<DIDDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int didId = -1;

            bool insertActionSucc = TryAddDID(did, out didId);

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

        public bool TryUpdateDID(DID did)
        {
            var cachedDIDs = GetCachedDIDsGroupByNumber();
            bool alreadyExist = false;
            DID tempDID;
            switch (did.DIDNumberType)
            {
                case DIDNumberType.Number:
                    foreach (string number in did.Settings.Numbers)
                    {
                        alreadyExist = cachedDIDs.TryGetValue(number, out tempDID) && tempDID.DIDId != did.DIDId;
                        if (alreadyExist)
                            break;
                    }
                    break;
                case DIDNumberType.Range:
                    foreach (DIDRange range in did.Settings.Ranges)
                    {
                        long from = range.From.TryParseWithValidate<long>(long.TryParse);
                        long to = range.To.TryParseWithValidate<long>(long.TryParse);

                        for (long index = from; index <= to; index++)
                        {
                            alreadyExist = cachedDIDs.TryGetValue(index.ToString(), out tempDID) && tempDID.DIDId != did.DIDId;
                            if (alreadyExist)
                                break;
                        }

                        if (alreadyExist)
                            break;
                    }
                    break;
                default: throw new Exception("Invalid Type for DID.");
            }
            if (!alreadyExist)
            {
                IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
                bool success = dataManager.Update(did);
                if (success)
                    VRActionLogger.Current.TrackAndLogObjectUpdated(DIDLoggableEntity.Instance, did);
                return success;
            }
            return false;
        }
        public UpdateOperationOutput<DIDDetail> UpdateDID(DID did)
        {
            UpdateOperationOutput<DIDDetail> updateOperationOutput = new UpdateOperationOutput<DIDDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (TryUpdateDID(did))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

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
                   Dictionary<string, DID> result = new Dictionary<string, DID>();

                   var dids = GetCachedDIDs();
                   foreach (var did in dids)
                   {
                       switch (did.Value.DIDNumberType)
                       {
                           case DIDNumberType.Number:
                               foreach (var number in did.Value.Settings.Numbers)
                               {
                                   result.Add(number, did.Value);
                               }
                               break;
                           case DIDNumberType.Range:
                               foreach (var range in did.Value.Settings.Ranges)
                               {
                                   long from = range.From.TryParseWithValidate<long>(long.TryParse);
                                   long to = range.To.TryParseWithValidate<long>(long.TryParse);

                                   for (long index = from; index <= to; index++)
                                   {
                                       result.Add(index.ToString(), did.Value);
                                   }
                               }
                               break;
                           default: throw new Exception("Invalid Type for DID.");
                       }
                   }
                   return result;
               });
        }
        public Dictionary<string, DID> GetCachedDIDsGroupBySourceId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDIDsGroupBySourceId",
               () =>
               {
                   var dids = GetCachedDIDs();
                   return dids.Where(v => !string.IsNullOrEmpty(v.Value.SourceId)).ToDictionary(kvp => kvp.Value.SourceId, kvp => kvp.Value); ;
               });
        }
        public Dictionary<int, DID> GetCachedDIDs()
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
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Description });
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
                return !string.IsNullOrEmpty(did.Description) ? did.Description : did.DIDId.ToString();
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
                Number = did.Description,
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
            return GetDIDNumberDescription(Int32.Parse(context.EntityId.ToString()));
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
