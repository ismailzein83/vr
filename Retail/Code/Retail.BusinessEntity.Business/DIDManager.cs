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
    public class DIDManager : BaseBusinessEntityManager
    {
        #region ctor/Local Variables

        Guid _accountDIDRelationDefinitionId;

        public DIDManager()
        {
            _accountDIDRelationDefinitionId = new ConfigManager().GetAccountDIDRelationDefinitionId();
        }

        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<DIDClientDetail> GetFilteredClientDIDs(Vanrise.Entities.DataRetrievalInput<DIDQuery> input)
        {
            var allDIDs = GetCachedDIDs();
            Func<DID, bool> filterExpression = (did) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Number) && !IsDIDContainNumber(did, input.Query.Number))
                    return false;
                if (input.Query.DIDNumberTypes != null && !input.Query.DIDNumberTypes.Contains(GetDIDNumberType(did)))
                    return false;

                if (!IsDIDAssignedToAccount(input.Query.AccountIds, did.DIDId, input.Query.WithSubAccounts))
                    return false;
                return true;
            };
            ResultProcessingHandler<DIDClientDetail> handler = new ResultProcessingHandler<DIDClientDetail>()
            {
                ExportExcelHandler = new DIDClientExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(DIDLoggableEntity.Instance, input);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allDIDs.ToBigResult(input, filterExpression, DIDClientDetailMapper), handler);
        }
        public Vanrise.Entities.IDataRetrievalResult<DIDDetail> GetFilteredDIDs(Vanrise.Entities.DataRetrievalInput<DIDQuery> input)
        {
            var allDIDs = GetCachedDIDs();

            Func<DID, bool> filterExpression = (did) =>
                {
                    if (!string.IsNullOrEmpty(input.Query.Number) && !IsDIDContainNumber(did, input.Query.Number))
                        return false;

                    if (input.Query.DIDNumberTypes != null && !input.Query.DIDNumberTypes.Contains(GetDIDNumberType(did)))
                        return false;

                    if (!IsDIDAssignedToAccount(input.Query.AccountIds, did.DIDId, input.Query.WithSubAccounts))
                        return false;

                    return true;
                };

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

        public DIDRuntimeEditor GetDIDRuntimeEditor(int didId)
        {
            DID did = GetDID(didId, true);
            return new DIDRuntimeEditor()
            {
                DID = did,
                Description = GetDIDDescription(did),
                DIDNumberType = GetDIDNumberType(did)
            };
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

        //public string GetDIDNumberDescription(int didId)
        //{
        //    var DIDs = GetCachedDIDs();
        //    DID did = DIDs.GetRecord(didId);
        //    if (did == null)
        //        throw new NullReferenceException(string.Format("DID ID {0}", didId));

        //    return GetDIDDescription(did);
        //}

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

        public InsertOperationOutput<DIDDetail> AddDID(DIDToInsert didToInsert)
        {
            InsertOperationOutput<DIDDetail> insertOperationOutput = new InsertOperationOutput<DIDDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int didId;
            if (TryAddDID(didToInsert, out didId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                didToInsert.DIDId = didId;

                if (!didToInsert.CreateAsSeparate)
                    insertOperationOutput.InsertedObject = DIDDetailMapper(didToInsert);

                VRActionLogger.Current.TrackAndLogObjectAdded(DIDLoggableEntity.Instance, didToInsert);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public bool TryAddDID(DIDToInsert didToInsert, out int didId)
        {
            didId = -1;
            bool alreadyExist = false;

            var cachedDIDs = GetCachedDIDsGroupByNumber();
            ManipulateDIDs(didToInsert, (number, handle) =>
            {
                alreadyExist = cachedDIDs.ContainsKey(number);
                if (alreadyExist)
                    handle.Stop = true;
            });

            if (!alreadyExist)
            {
                IDIDDataManager dataManager = BEDataManagerFactory.GetDataManager<IDIDDataManager>();
                DIDNumberType didNumberType = GetDIDNumberType(didToInsert);

                if (didNumberType == DIDNumberType.Range && didToInsert.CreateAsSeparate)
                {
                    List<DID> dids = CreateRangeAsSeparateDIDs(didToInsert);
                    return dids != null ? dataManager.Insert(dids) : true;
                }
                else
                {
                    return dataManager.Insert(didToInsert, out didId);
                }
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

        public bool TryUpdateDID(DID did)
        {
            var cachedDIDs = GetCachedDIDsGroupByNumber();
            bool alreadyExist = false;
            DID tempDID;
            ManipulateDIDs(did, (number, handle) =>
            {
                alreadyExist = cachedDIDs.TryGetValue(number, out tempDID) && tempDID.DIDId != did.DIDId;
                if (alreadyExist)
                    handle.Stop = true;
            });

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

        public bool IsDIDContainNumber(DID did, string number)
        {
            did.ThrowIfNull("did");
            did.Settings.ThrowIfNull("did.Settings");

            if (string.IsNullOrEmpty(number))
                return true;

            string lowerNumber = number.ToLower();

            if (did.Settings.Numbers != null && did.Settings.Numbers.Count > 0)
            {
                if (did.Settings.Numbers.Select(itm => itm.ToLower()).Contains(lowerNumber))
                    return true;
            }

            if (did.Settings.Ranges != null && did.Settings.Ranges.Count > 0)
            {
                long numberAsLong = number.TryParseWithValidate<long>(long.TryParse);
                foreach (DIDRange range in did.Settings.Ranges)
                {
                    long from = range.From.TryParseWithValidate<long>(long.TryParse);
                    long to = range.To.TryParseWithValidate<long>(long.TryParse);
                    if (numberAsLong >= from && numberAsLong <= to)
                        return true;
                }
            }

            return false;
        }

        public string GetDIDDescription(int didId)
        {
            var cachedDIDs = GetCachedDIDs();
            DID did = cachedDIDs.GetRecord(didId);
            return GetDIDDescription(did);
        }

        public string GetDIDDescription(DID did)
        {
            DIDNumberType didNumberType = GetDIDNumberType(did);
            switch (didNumberType)
            {
                case Entities.DIDNumberType.Number:
                    return string.Format("{0}", string.Join<string>(", ", did.Settings.Numbers));

                case Entities.DIDNumberType.Range:
                    return string.Format("{0}", string.Join<string>(", ", did.Settings.Ranges.Select(itm => string.Format("{0} -> {1}", itm.From, itm.To))));

                default: throw new Exception("Invalid Type for DID.");
            }
        }

        public DIDNumberType GetDIDNumberType(int didId)
        {
            var cachedDIDs = GetCachedDIDs();
            DID did = cachedDIDs.GetRecord(didId);
            return GetDIDNumberType(did);
        }

        public DIDNumberType GetDIDNumberType(DID did)
        {
            did.ThrowIfNull("did");
            did.Settings.ThrowIfNull("did.Settings");

            if (did.Settings.Numbers != null && did.Settings.Numbers.Count > 0)
                return Entities.DIDNumberType.Number;

            if (did.Settings.Ranges != null && did.Settings.Ranges.Count > 0)
                return Entities.DIDNumberType.Range;

            throw new Exception("Invalid Type for DID.");
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

        public BEParentChildRelationDefinition GetAccountDIDRelationDefinition()
        {
            var beParentChildRelationDefinition = new BEParentChildRelationDefinitionManager().GetBEParentChildRelationDefinition(_accountDIDRelationDefinitionId);
            return beParentChildRelationDefinition;
        }

        public void ManipulateDIDs(DID did, Action<string, DIDNumberHandle> onNumberLoaded)
        {
            did.ThrowIfNull("did");
            did.Settings.ThrowIfNull("did.Settings");

            DIDNumberType didNumberType = GetDIDNumberType(did);
            switch (didNumberType)
            {
                case DIDNumberType.Number:
                    did.Settings.Numbers.ThrowIfNull("did.Settings.Numbers");
                    foreach (var number in did.Settings.Numbers)
                    {
                        DIDNumberHandle handle = new DIDNumberHandle();
                        onNumberLoaded(number, handle);
                        if (handle.Stop)
                            break;
                    }
                    break;

                case DIDNumberType.Range:
                    did.Settings.Ranges.ThrowIfNull("did.Settings.Ranges", did.DIDId);
                    foreach (var range in did.Settings.Ranges)
                    {
                        DIDNumberHandle handle = new DIDNumberHandle();
                        long from = range.From.TryParseWithValidate<long>(long.TryParse);
                        long to = range.To.TryParseWithValidate<long>(long.TryParse);

                        for (long index = from; index <= to; index++)
                        {
                            onNumberLoaded(index.ToString(), handle);
                            if (handle.Stop)
                                break;
                        }
                        if (handle.Stop)
                            break;
                    }
                    break;

                default: throw new Exception("Invalid Type for DID.");
            }
        }

        public List<string> GetAllDIDNumbers(DID did)
        {
            List<string> numbers = new List<string>();
            ManipulateDIDs(did, (number, handler) => numbers.Add(number));
            return numbers;
        }

        public Dictionary<long, DIDAccount> GetAllDIDAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<BEParentChildRelationManager.CacheManager>().GetOrCreateObject("Retail_DIDManager_GetAllDIDAccounts",
                _accountDIDRelationDefinitionId,
                () =>
                {
                    var relationDefinition = GetAccountDIDRelationDefinition();
                    relationDefinition.ThrowIfNull("relationDefinition");
                    relationDefinition.Settings.ThrowIfNull("relationDefinition.Settings");
                    Dictionary<long, DIDAccount> allDIDAcccounts = new Dictionary<long, DIDAccount>();
                    var parentChildRelationManager = new BEParentChildRelationManager();
                    IEnumerable<BEParentChildRelation> allRelations = parentChildRelationManager.GetBEParentChildRelationsByDefinitionId(_accountDIDRelationDefinitionId);
                    var accountBEManager = new AccountBEManager();
                    if (allRelations != null)
                    {
                        foreach (var relation in allRelations)
                        {
                            int didId = relation.ChildBEId.TryParseWithValidate<int>(int.TryParse, "relation.ChildBEId");
                            long accountId = relation.ParentBEId.TryParseWithValidate<long>(long.TryParse, "relation.ParentBEId");
                            var did = GetDID(didId);
                            did.ThrowIfNull("did", didId);
                            var account = accountBEManager.GetAccount(relationDefinition.Settings.ParentBEDefinitionId, accountId);
                            account.ThrowIfNull("account", accountId);
                            var didAccount = new DIDAccount
                                {
                                    DIDAccountId = relation.BEParentChildRelationId,
                                    DID = did,
                                    Account = account,
                                    BED = relation.BED,
                                    EED = relation.EED
                                };
                            allDIDAcccounts.Add(didAccount.DIDAccountId, didAccount);
                        }
                    }
                    return allDIDAcccounts;
                });
        }

        public Dictionary< long, List<DIDAccount>> GetDIDAccountsByAccountId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<BEParentChildRelationManager.CacheManager>().GetOrCreateObject("Retail_DIDManager_GetDIDAccountsByAccountId",
                _accountDIDRelationDefinitionId,
                () =>
                {
                    Dictionary<long, List<DIDAccount>> didAccountsByAccountId = new Dictionary<long, List<DIDAccount>>();
                    var allDIDAccounts = GetAllDIDAccounts();
                    if(allDIDAccounts != null)
                    {
                        foreach(var didAccount in allDIDAccounts.Values)
                        {
                            didAccountsByAccountId.GetOrCreateItem(didAccount.Account.AccountId).Add(didAccount);
                        }
                    }
                    return didAccountsByAccountId;
                });
        }

        public Dictionary<int, List<DIDAccount>> GetDIDAccountsByDIDId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<BEParentChildRelationManager.CacheManager>().GetOrCreateObject("Retail_DIDManager_GetDIDAccountsByDIDId",
                _accountDIDRelationDefinitionId,
                () =>
                {
                    Dictionary<int, List<DIDAccount>> didAccountsByDID = new Dictionary<int, List<DIDAccount>>();
                    var allDIDAccounts = GetAllDIDAccounts();
                    if (allDIDAccounts != null)
                    {
                        foreach (var didAccount in allDIDAccounts.Values)
                        {
                            didAccountsByDID.GetOrCreateItem(didAccount.DID.DIDId).Add(didAccount);
                        }
                    }
                    return didAccountsByDID;
                });
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
                       ManipulateDIDs(did.Value, (number, handle) => { result.Add(number, did.Value); });
                   return result;
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

        private bool IsDIDAssignedToAccount(List<long> accountIds, int didId, bool withSubAccounts)
        {
            if (accountIds == null || accountIds.Count == 0)
                return true;

            BEParentChildRelation beParentChildRelation = new BEParentChildRelationManager().GetParent(_accountDIDRelationDefinitionId, didId.ToString(), DateTime.Now);
            if (beParentChildRelation == null)
                return false;
            List<long> filteredAccountIds = new List<long>();
            if(withSubAccounts)
            {
                var beParentChildRelationDefinitionManager = new BEParentChildRelationDefinitionManager();
                var definition = beParentChildRelationDefinitionManager.GetBEParentChildRelationDefinition(_accountDIDRelationDefinitionId);
                var accountBEDefinitionId = definition.Settings.ParentBEDefinitionId;
                AccountBEManager accountBEManager = new AccountBEManager();
                foreach(var accountId in accountIds)
                {
                    filteredAccountIds.Add(accountId);
                    var childAccountIds = accountBEManager.GetChildAccountIds(accountBEDefinitionId, accountId, true);
                    if (childAccountIds != null)
                        filteredAccountIds.AddRange(childAccountIds);
                }
            }else
            {
                filteredAccountIds = accountIds;
            }
            return filteredAccountIds.Contains(long.Parse(beParentChildRelation.ParentBEId));
        }

        private List<DID> CreateRangeAsSeparateDIDs(DID didToInsert)
        {
            didToInsert.ThrowIfNull("didToInsert");
            didToInsert.Settings.ThrowIfNull("didToInsert.Settings");
            didToInsert.Settings.Ranges.ThrowIfNull("didToInsert.Settings.Numbers");

            List<DID> dids = new List<DID>();

            foreach (var range in didToInsert.Settings.Ranges)
            {
                long from = range.From.TryParseWithValidate<long>(long.TryParse);
                long to = range.To.TryParseWithValidate<long>(long.TryParse);

                for (long currentNumber = from; currentNumber <= to; currentNumber++)
                {
                    List<string> numbers = new List<string>();
                    numbers.Add(currentNumber.ToString());

                    DID did = new DID()
                    {
                        DIDId = didToInsert.DIDId,
                        SourceId = didToInsert.SourceId,
                        Settings = new DIDSettings()
                        {
                            Numbers = numbers,
                            Ranges = null,
                            NumberOfChannels = didToInsert.Settings.NumberOfChannels,
                            DIDSo = didToInsert.Settings.DIDSo,
                            IsInternational = didToInsert.Settings.IsInternational
                        }
                    };
                    dids.Add(did);
                }
            }

            return dids.Count > 0 ? dids : null;
        }

        #endregion

        #region Private Classes

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
                return s_didManager.GetDIDDescription(did);
            }
        }

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
                    DIDManager didManager = new DIDManager();
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null && record.Entity.Settings != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.AccountName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.NumberOfChannels });
                            row.Cells.Add(new ExportExcelCell { Value = didManager.GetDIDDescription(record.Entity) });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.IsInternational });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class DIDClientExcelExportHandler : ExcelExportHandler<DIDClientDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DIDClientDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "DIDs",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Account" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Number" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    DIDManager didManager = new DIDManager();
                    foreach (var record in context.BigResult.Data)
                    {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.AccountName });
                            row.Cells.Add(new ExportExcelCell { Value =record.Description });
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion

        #region  Mappers

        private DIDDetail DIDDetailMapper(DID did)
        {
            DIDDetail didDetail = new DIDDetail();
            didDetail.Entity = did;
            didDetail.Description = GetDIDDescription(did);

            BEParentChildRelation beParentChildRelation = new BEParentChildRelationManager().GetParent(_accountDIDRelationDefinitionId, did.DIDId.ToString(), DateTime.Now);
            if (beParentChildRelation != null)
            {
                BEParentChildRelationDefinition beParentChildRelationDefinition = new BEParentChildRelationDefinitionManager().GetBEParentChildRelationDefinition(_accountDIDRelationDefinitionId);
                didDetail.AccountName = new BusinessEntityManager().GetEntityDescription(beParentChildRelationDefinition.Settings.ParentBEDefinitionId, long.Parse(beParentChildRelation.ParentBEId));
            }
            return didDetail;
        }
        private DIDClientDetail DIDClientDetailMapper(DID did)
        {
            DIDClientDetail didClientDetail = new DIDClientDetail();
            didClientDetail.Description = GetDIDDescription(did);
            BEParentChildRelation beParentChildRelation = new BEParentChildRelationManager().GetParent(_accountDIDRelationDefinitionId, did.DIDId.ToString(), DateTime.Now);
            if (beParentChildRelation != null)
            {
                BEParentChildRelationDefinition beParentChildRelationDefinition = new BEParentChildRelationDefinitionManager().GetBEParentChildRelationDefinition(_accountDIDRelationDefinitionId);
                didClientDetail.AccountName = new BusinessEntityManager().GetEntityDescription(beParentChildRelationDefinition.Settings.ParentBEDefinitionId, long.Parse(beParentChildRelation.ParentBEId));
            }
            return didClientDetail;
        }
        private DIDInfo DIDInfoMapper(DID did)
        {
            return new DIDInfo()
            {
                DIDId = did.DIDId,
                Number = GetDIDDescription(did)
            };
        }

        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetCachedDIDs().Select(itm => itm as dynamic).ToList();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetDID(context.EntityId);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetDIDDescription(Int32.Parse(context.EntityId.ToString()));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var did = context.Entity as DID;
            return did.DIDId;
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DIDNumberHandle
    {
        public bool Stop { get; set; }
    }
}
