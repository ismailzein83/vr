using Retail.BusinessEntity.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Retail.Teles.Business.AccountBEActionTypes;
using System.Security.Policy;
using Retail.Teles.Business.Provisioning;
using Retail.Teles.Data;

namespace Retail.Teles.Business
{
    public class TelesEnterpriseManager : BaseBusinessEntityManager
    {
        #region Public Methods
        AccountBEManager _accountBEManager = new AccountBEManager();

        public IDataRetrievalResult<EnterpriseDIDDetail> GetFilteredEnterpriseDIDs(DataRetrievalInput<EnterpriseDIDsQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new EnterpriseDIDsRequestHandler());
        }
        public IDataRetrievalResult<EnterpriseBusinessTrunkDetail> GetFilteredEnterpriseBusinessTrunks(DataRetrievalInput<EnterpriseBusinessTrunksQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new EnterpriseBusinessTrunksRequestHandler());
        }
        public IDataRetrievalResult<AccountEnterpriseDIDDetail> GetFilteredAccountEnterprisesDIDs(DataRetrievalInput<AccountEnterpriseDIDQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new AccountEnterpriseDIDRequestHandler());
        }

        public void SaveAccountEnterprisesDIDs()
        {
            var accountEnterprisesDIDs = GetAccountEnterprisesDIDs();
            ITelesAccountEnterpriseDataManager dataManager = TelesDataManagerFactory.GetDataManager<ITelesAccountEnterpriseDataManager>();
            dataManager.SaveAccountEnterprisesDIDs(accountEnterprisesDIDs);
        }
        public IEnumerable<TelesEnterpriseInfo> GetEnterprisesInfo(Guid vrConnectionId, TelesEnterpriseFilter filter)
        {
            var cachedEnterprises = GetCachedEnterprises(vrConnectionId, filter.TelesDomainId, false);

            Func<TelesEnterpriseInfo, bool> filterFunc = null;
            if (filter != null)
            {
                filterFunc = (telesEnterpriseInfo) =>
                {

                    if (filter.Filters != null)
                    {
                        var context = new TelesEnterpriseFilterContext() { EnterpriseId = telesEnterpriseInfo.TelesEnterpriseId, AccountBEDefinitionId = filter.AccountBEDefinitionId };
                        if (filter.Filters.Any(x => x.IsExcluded(context)))
                            return false;
                    }
                    return true;
                };
                return cachedEnterprises.FindAllRecords(filterFunc).OrderBy(x => x.Name);

            }

            return cachedEnterprises.Values.OrderBy(x => x.Name);
        }
        public TelesEnterpriseInfo GetEnterprise(Guid vrConnectionId, string enterpriseId, string telesDomainId)
        {
            var cachedEnterprises = GetCachedEnterprises(vrConnectionId, telesDomainId, false);
            TelesEnterpriseInfo enterpriseInfo;
            cachedEnterprises.TryGetValue(enterpriseId, out enterpriseInfo);
            return enterpriseInfo;
        }
        public string CreateEnterprise(Guid vrConnectionId, string centrexFeatSet, Enterprise request)
        {
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            var actionPath = string.Format("/domain/{0}?centrexFeatSet={1}", telesRestConnection.DefaultDomainId, centrexFeatSet);
            VRWebAPIResponse<string> response = telesRestConnection.Post<Enterprise, string>(actionPath, request, true);
            response.Headers.Location.ThrowIfNull("response.Headers", response.Headers);
            var enterpriseId = response.Headers.Location.Segments.Last();
            enterpriseId.ThrowIfNull("enterpriseId", enterpriseId);
            TelesEnterpriseManager.SetCacheExpired();
            return Convert.ToString(enterpriseId);
        }
        public string GetEnterpriseName(Guid vrConnectionId, string enterpriseId, string telesDomainId)
        {
            var cachedEnterprises = GetCachedEnterprises(vrConnectionId, telesDomainId, true);
            if (cachedEnterprises != null)
            {
                TelesEnterpriseInfo enterpriseInfo;
                if (cachedEnterprises.TryGetValue(enterpriseId, out enterpriseInfo))
                    return enterpriseInfo.Name;
                else
                    return null;
            }
            else
            {
                return string.Format("{0} (Name unavailable)", enterpriseId);
            }
        }
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> MapEnterpriseToAccount(MapEnterpriseToAccountInput input)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (CanMapTelesEnterprise(input.AccountBEDefinitionId, input.TelesEnterpriseId) && IsMapEnterpriseToAccountValid(input.AccountBEDefinitionId, input.AccountId, input.ActionDefinitionId))
            {
                bool result = TryMapEnterpriseToAccount(input.AccountBEDefinitionId, input.AccountId, input.TelesEnterpriseId);
                if (result)
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    _accountBEManager.TrackAndLogObjectCustomAction(input.AccountBEDefinitionId, input.AccountId, "Map To Teles Enterprise", null, null);
                    updateOperationOutput.UpdatedObject = _accountBEManager.GetAccountDetail(input.AccountBEDefinitionId, input.AccountId);
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }

            }
            return updateOperationOutput;

        }

        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UnmapEnterpriseToAccount(TelesAccountToUnmap input)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool result = TryUnMapEnterpriseToAccount(input.AccountBEDefinitionId, input.AccountId);
            if (result)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                _accountBEManager.TrackAndLogObjectCustomAction(input.AccountBEDefinitionId, input.AccountId, "Unmap Teles Enterprise", null, null);
                updateOperationOutput.UpdatedObject = _accountBEManager.GetAccountDetail(input.AccountBEDefinitionId, input.AccountId);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            
            return updateOperationOutput;
        }

        public bool CanMapTelesEnterprise(Guid accountBEDefinitionId, string telesEnterpriseId)
        {
            var cachedAccountsByEnterprises = GetCachedAccountsByEnterprises(accountBEDefinitionId);
            if (cachedAccountsByEnterprises != null && cachedAccountsByEnterprises.ContainsKey(telesEnterpriseId))
                return false;
            return true;
        }

        public bool TryMapEnterpriseToAccount(Guid accountBEDefinitionId, long accountId, string telesEnterpriseId, ProvisionStatus? status = null)
        {

            EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = new EnterpriseAccountMappingInfo { TelesEnterpriseId = telesEnterpriseId, Status = status };

            //var accountChilds = _accountBEManager.GetChildAccounts(accountBEDefinitionId, accountId, false);
            //foreach(var account in accountChilds)
            //{
            //    _accountBEManager.DeleteAccountExtendedSetting<SiteAccountMappingInfo>(accountBEDefinitionId, account.AccountId);
            //}

            return _accountBEManager.UpdateAccountExtendedSetting<EnterpriseAccountMappingInfo>(accountBEDefinitionId, accountId,
                enterpriseAccountMappingInfo);
        }

        public bool TryUnMapEnterpriseToAccount(Guid accountBEDefinitionId, long accountId)
        {
            return _accountBEManager.DeleteAccountExtendedSetting<EnterpriseAccountMappingInfo>(accountBEDefinitionId, accountId);
        }

        public bool IsMapEnterpriseToAccountValid(Guid accountBEDefinitionId, long accountId, Guid actionDefinitionId)
        {
            var accountDefinitionAction = new AccountBEDefinitionManager().GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId);
            if (accountDefinitionAction != null)
            {
                var settings = accountDefinitionAction.ActionDefinitionSettings as MappingTelesAccountActionSettings;
                if (settings != null)
                {
                    var account = _accountBEManager.GetAccount(accountBEDefinitionId, accountId);
                    return _accountBEManager.EvaluateAccountCondition(account, accountDefinitionAction.AvailabilityCondition);
                }

            }
            return false;
        }

        public Dictionary<string, long> GetCachedAccountsByEnterprises(Guid accountBEDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>().GetOrCreateObject("GetCachedAccountsByEnterprises", accountBEDefinitionId, () =>
            {
                var accountBEManager = new AccountBEManager();
                var cashedAccounts = accountBEManager.GetAccounts(accountBEDefinitionId);
                Dictionary<string, long> accountsByEnterprises = null;
                foreach (var item in cashedAccounts)
                {
                    var enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(item.Value);
                    if (enterpriseAccountMappingInfo != null)
                    {
                        if (accountsByEnterprises == null)
                            accountsByEnterprises = new Dictionary<string, long>();
                        accountsByEnterprises.Add(enterpriseAccountMappingInfo.TelesEnterpriseId, item.Key);
                    }
                }
                return accountsByEnterprises;
            });
        }
        public Dictionary<long, string> GetCachedEnterprisesByAccounts(Guid accountBEDefinitionId)
        {

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>().GetOrCreateObject(string.Format("GetCachedEnterprisesByAccounts_{0}", accountBEDefinitionId), accountBEDefinitionId, () =>
            {
                var accountBEManager = new AccountBEManager();
                var cashedAccounts = accountBEManager.GetAccounts(accountBEDefinitionId);
                Dictionary<long, string> enterprisesByAccounts = null;
                foreach (var item in cashedAccounts)
                {
                    var enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(item.Value);
                    if (enterpriseAccountMappingInfo != null)
                    {
                        if (enterprisesByAccounts == null)
                            enterprisesByAccounts = new Dictionary<long, string>();
                        enterprisesByAccounts.Add(item.Key, enterpriseAccountMappingInfo.TelesEnterpriseId);
                    }
                }
                return enterprisesByAccounts;
            });
        }
        public string GetParentAccountEnterpriseId(Guid accountBEDefinitionId, long accountId)
        {
            var parentAccount = _accountBEManager.GetParentAccount(accountBEDefinitionId, accountId);
            parentAccount.ThrowIfNull("parentAccount", accountId);
            EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(parentAccount);
            enterpriseAccountMappingInfo.ThrowIfNull("enterpriseAccountMappingInfo", accountId);
            return enterpriseAccountMappingInfo.TelesEnterpriseId;
        }
        public bool DoesUserHaveExecutePermission(Guid accountBEDefinitionId)
        {
            var accountDefinitionActions = new AccountBEDefinitionManager().GetAccountActionDefinitions(accountBEDefinitionId);
            foreach (var a in accountDefinitionActions)
            {
                var settings = a.ActionDefinitionSettings as MappingTelesAccountActionSettings;
                if (settings != null)
                    return settings.DoesUserHaveExecutePermission();
            }
            return false;
        }
        public List<AccountEnterpriseDID> GetAccountEnterprisesDIDs()
        {
            Guid vrConnectionId = Guid.Parse("93035c7d-8334-4434-a0d6-a9691951668c");
            Guid accountBEDefinitionId = Guid.Parse("9a427357-cf55-4f33-99f7-745206dee7cd");
            string countryCode = "965";

            AccountBEManager accountBEManager = new AccountBEManager();
            var accountByEnterpriseId = new TelesEnterpriseManager().GetCachedAccountsByEnterprises(accountBEDefinitionId);

            List<AccountEnterpriseDID> accountEnterprisesDIDs = new List<AccountEnterpriseDID>();
            var domainLists = GetDomainLists(vrConnectionId);
            Dictionary<string, SiteEntity> sites = new Dictionary<string, SiteEntity>();
            Dictionary<string, EnterpriseEntity> enterprises = new Dictionary<string, EnterpriseEntity>();
            string level;
            if (domainLists != null)
            {
                foreach (var domainList in domainLists)
                {
                    level = domainList.level.ToString();
                    if (!string.IsNullOrEmpty(level))
                    {
                        if (level.Equals("Site", StringComparison.OrdinalIgnoreCase))
                        {
                            sites.Add(domainList.id.ToString(), new SiteEntity()
                            {
                                Name = domainList.name.ToString(),
                                Description = domainList.description.ToString(),
                            });
                        }
                        else if (level.Equals("Enterprise", StringComparison.OrdinalIgnoreCase))
                        {
                            enterprises.Add(domainList.id.ToString(), new EnterpriseEntity()
                            {
                                Name = domainList.name.ToString(),
                                Description = domainList.description.ToString(),
                            });
                        }
                    }
                }
            }

            var screenNumbers = GetScreenNumbersByCountryCode(vrConnectionId, countryCode);
            if (screenNumbers != null)
            {
                foreach (var screenNumber in screenNumbers)
                {
                    string enterpriseId = screenNumber.domainId.ToString();
                    string siteId = screenNumber.siteId != null ? screenNumber.siteId.ToString() : string.Empty;

                    long? accountId = null;
                    long enterpriseAccountId;

                    EnterpriseEntity enterpriseEntity = enterprises.GetRecord(enterpriseId);
                    SiteEntity siteEntity = sites.GetRecord(siteId);

                    if (accountByEnterpriseId.TryGetValue(enterpriseId, out enterpriseAccountId))
                        accountId = enterpriseAccountId;

                    accountEnterprisesDIDs.Add(new AccountEnterpriseDID()
                    {
                        AccountId = accountId,
                        ScreenNumber = screenNumber.sn.ToString(),
                        EnterpriseId = enterpriseId,
                        EnterpriseName = enterpriseEntity != null ? enterpriseEntity.Name : null,
                        EnterpriseDescription = enterpriseEntity != null ? enterpriseEntity.Description : null,
                        SiteId = siteId,
                        SiteName = siteEntity != null ? siteEntity.Name : null,
                        SiteDescription = siteEntity != null ? siteEntity.Description : null,
                        MaxCalls = 1,
                        Type = AccountEnterpriseDIDType.SN
                    });
                }
            }



            var businessTrunkReports = GetUserBTReports(vrConnectionId);

            if (businessTrunkReports != null)
            {
                foreach (var businessTrunkReport in businessTrunkReports)
                {
                    string domainId = businessTrunkReport.user.domainId.ToString();
                    string enterpriseId = businessTrunkReport.user.enterpriseId.ToString();
                    string siteId = businessTrunkReport.user.siteId != null ? businessTrunkReport.user.siteId.ToString() : string.Empty;


                    long? accountId = null;
                    long enterpriseAccountId;
                    if (accountByEnterpriseId.TryGetValue(domainId, out enterpriseAccountId))
                        accountId = enterpriseAccountId;

                    EnterpriseEntity enterpriseEntity = enterprises.GetRecord(enterpriseId);
                    string enterpriseName = enterpriseEntity != null ? enterpriseEntity.Name : null;
                    string enterpriseDescription = enterpriseEntity != null ? enterpriseEntity.Description : null;

                    SiteEntity siteEntity = sites.GetRecord(siteId);
                    string siteName = siteEntity != null ? siteEntity.Name : null;
                    string siteDescription = siteEntity != null ? siteEntity.Description : null;

                    string screenNumber = null;
                    if (businessTrunkReport.numberRanges != null)
                    {
                        foreach (var numberRange in businessTrunkReport.numberRanges)
                        {
                            int maxCalls = businessTrunkReport.specialTarget.maxCalls;
                            if (numberRange.systemRange == true)
                            {
                                screenNumber = numberRange.startSn.ToString();
                            }
                            var startSn = numberRange.startSn.ToString();
                            accountEnterprisesDIDs.Add(new AccountEnterpriseDID()
                            {
                                AccountId = accountId,
                                ScreenNumber = startSn,
                                EnterpriseId = enterpriseId,
                                EnterpriseName = enterpriseName,
                                EnterpriseDescription = enterpriseDescription,
                                SiteId = siteId,
                                SiteName = siteName,
                                SiteDescription = siteDescription,
                                MaxCalls = maxCalls,
                                Type = AccountEnterpriseDIDType.BT
                            });
                            if (numberRange.endSn != null)
                            {
                                long parsedStartSn;
                                long parsedEndSn = -1;
                                var endSn = numberRange.endSn.ToString();
                                if (long.TryParse(startSn, out parsedStartSn) && long.TryParse(endSn, out parsedEndSn))
                                {
                                    long i = parsedStartSn + 1;
                                    while (i <= parsedEndSn)
                                    {
                                        accountEnterprisesDIDs.Add(new AccountEnterpriseDID()
                                        {
                                            AccountId = accountId,
                                            ScreenNumber = i.ToString(),
                                            EnterpriseId = enterpriseId,
                                            EnterpriseName = enterpriseName,
                                            EnterpriseDescription = enterpriseDescription,
                                            SiteId = siteId,
                                            SiteName = siteName,
                                            SiteDescription = siteDescription,
                                            MaxCalls = maxCalls,
                                            Type = AccountEnterpriseDIDType.BT
                                        });
                                        i++;
                                    }
                                }

                            }
                        }
                    }
                }
            }

            return accountEnterprisesDIDs;
        }
        public static void SetCacheExpired()
        {
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get
                {
                    return true;
                }
            }
            protected override bool UseCentralizedCacheRefresher
            {
                get
                {
                    return true;
                }
            }

        }

        private class CachedEnterprisesInfo
        {
            public bool IsValid { get; set; }

            public Dictionary<string, TelesEnterpriseInfo> EnterpriseInfos { get; set; }
        }

        private class EnterpriseDIDsRequestHandler : BigDataRequestHandler<EnterpriseDIDsQuery, EnterpriseDID, EnterpriseDIDDetail>
        {
            public EnterpriseDIDsRequestHandler()
            {

            }
            public override EnterpriseDIDDetail EntityDetailMapper(EnterpriseDID entity)
            {
                return new EnterpriseDIDDetail
                {
                    ScreenNumber = entity.ScreenNumber
                };
            }
            protected override Vanrise.Entities.BigResult<EnterpriseDIDDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<EnterpriseDIDsQuery> input, IEnumerable<EnterpriseDID> allRecords)
            {
                return allRecords.ToBigResult(input, null, EntityDetailMapper);
            }
            public override IEnumerable<EnterpriseDID> RetrieveAllData(DataRetrievalInput<EnterpriseDIDsQuery> input)
            {
                AccountBEManager accountBEManager = new AccountBEManager();
                EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(input.Query.AccountBEDefinitionId, input.Query.AccountId);
                List<EnterpriseDID> enterpriseDID = new List<EnterpriseDID>();
                if (enterpriseAccountMappingInfo != null && enterpriseAccountMappingInfo.TelesEnterpriseId != null)
                {
                    string actionPath = string.Format("/screenNum/search?entId={0}", enterpriseAccountMappingInfo.TelesEnterpriseId);

                    VRConnectionManager vrConnectionManager = new VRConnectionManager();
                    VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(input.Query.VRConnectionId);
                    TelesRestConnection telesRestConnection = vrConnection.Settings.CastWithValidate<TelesRestConnection>("telesRestConnection", input.Query.VRConnectionId);
                    List<dynamic> telesDids = telesRestConnection.Get<List<dynamic>>(actionPath);
                    if (telesDids != null)
                    {
                        foreach (var item in telesDids)
                        {
                            enterpriseDID.Add(new EnterpriseDID
                            {
                                ScreenNumber = item.sn
                            });
                        }
                    }
                }
                return enterpriseDID;
            }

            protected override ResultProcessingHandler<EnterpriseDIDDetail> GetResultProcessingHandler(DataRetrievalInput<EnterpriseDIDsQuery> input, BigResult<EnterpriseDIDDetail> bigResult)
            {
                return new ResultProcessingHandler<EnterpriseDIDDetail>
                {
                    ExportExcelHandler = new EnterpriseDIDsExcelExportHandler(input.Query)
                };
            }
        }
        private class EnterpriseDIDsExcelExportHandler : ExcelExportHandler<EnterpriseDIDDetail>
        {
            EnterpriseDIDsQuery _query;
            public EnterpriseDIDsExcelExportHandler(EnterpriseDIDsQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<EnterpriseDIDDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "DIDs",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell> { new ExportExcelHeaderCell { Title = "Screen Number" } } }
                };

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var results = context.BigResult as BigResult<EnterpriseDIDDetail>;

                    if (results != null && results.Data != null)
                    {
                        sheet.Rows = new List<ExportExcelRow>();
                        foreach (var item in results.Data)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell { Value = item.ScreenNumber });
                            sheet.Rows.Add(row);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class EnterpriseBusinessTrunksRequestHandler : BigDataRequestHandler<EnterpriseBusinessTrunksQuery, EnterpriseBusinessTrunk, EnterpriseBusinessTrunkDetail>
        {
            public EnterpriseBusinessTrunksRequestHandler()
            {

            }
            public override EnterpriseBusinessTrunkDetail EntityDetailMapper(EnterpriseBusinessTrunk entity)
            {
                return new EnterpriseBusinessTrunkDetail
                {
                    StartSn = entity.StartSn
                };
            }
            protected override Vanrise.Entities.BigResult<EnterpriseBusinessTrunkDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<EnterpriseBusinessTrunksQuery> input, IEnumerable<EnterpriseBusinessTrunk> allRecords)
            {
                return allRecords.ToBigResult(input, null, EntityDetailMapper);
            }
            public override IEnumerable<EnterpriseBusinessTrunk> RetrieveAllData(DataRetrievalInput<EnterpriseBusinessTrunksQuery> input)
            {
                AccountBEManager accountBEManager = new AccountBEManager();
                EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(input.Query.AccountBEDefinitionId, input.Query.AccountId);
                List<EnterpriseBusinessTrunk> enterpriseBusinessTrunks = new List<EnterpriseBusinessTrunk>();

                if (enterpriseAccountMappingInfo != null && enterpriseAccountMappingInfo.TelesEnterpriseId != null)
                {
                    var telesSites = new TelesSiteManager().GetSites(input.Query.VRConnectionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
                    if (telesSites != null)
                    {
                        VRConnectionManager vrConnectionManager = new VRConnectionManager();
                        VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(input.Query.VRConnectionId);
                        TelesRestConnection telesRestConnection = vrConnection.Settings.CastWithValidate<TelesRestConnection>("telesRestConnection", input.Query.VRConnectionId);

                        foreach (var site in telesSites)
                        {
                            string actionPath = string.Format("/domain/{0}/user", site.id);
                            List<dynamic> teleUsers = telesRestConnection.Get<List<dynamic>>(actionPath);
                            if (teleUsers != null)
                            {
                                foreach (var teleUser in teleUsers)
                                {
                                    if (teleUser.numberRanges != null)
                                    {
                                        foreach (var numberRange in teleUser.numberRanges)
                                        {
                                            enterpriseBusinessTrunks.Add(new EnterpriseBusinessTrunk
                                            {
                                                StartSn = numberRange.startSn
                                            });
                                        }
                                    }

                                }
                            }

                        }
                    }

                }

                return enterpriseBusinessTrunks;
            }
            protected override ResultProcessingHandler<EnterpriseBusinessTrunkDetail> GetResultProcessingHandler(DataRetrievalInput<EnterpriseBusinessTrunksQuery> input, BigResult<EnterpriseBusinessTrunkDetail> bigResult)
            {
                return new ResultProcessingHandler<EnterpriseBusinessTrunkDetail>
                {
                    ExportExcelHandler = new EnterpriseBusinessTrunkExcelExportHandler(input.Query)
                };
            }
        }
        private class EnterpriseBusinessTrunkExcelExportHandler : ExcelExportHandler<EnterpriseBusinessTrunkDetail>
        {
            EnterpriseBusinessTrunksQuery _query;
            public EnterpriseBusinessTrunkExcelExportHandler(EnterpriseBusinessTrunksQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<EnterpriseBusinessTrunkDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Business Trunks",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell> { new ExportExcelHeaderCell { Title = "Business Trunk" } } }
                };

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var results = context.BigResult as BigResult<EnterpriseBusinessTrunkDetail>;

                    if (results != null && results.Data != null)
                    {
                        sheet.Rows = new List<ExportExcelRow>();
                        foreach (var item in results.Data)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell { Value = item.StartSn });
                            sheet.Rows.Add(row);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }


        private class AccountEnterpriseDIDRequestHandler : BigDataRequestHandler<AccountEnterpriseDIDQuery, AccountEnterpriseDID, AccountEnterpriseDIDDetail>
        {
            public AccountEnterpriseDIDRequestHandler()
            {

            }
            public override AccountEnterpriseDIDDetail EntityDetailMapper(AccountEnterpriseDID entity)
            {
                Guid accountBEDefinitionId = Guid.Parse("9a427357-cf55-4f33-99f7-745206dee7cd");
                Guid vrConnectionId = Guid.Parse("93035c7d-8334-4434-a0d6-a9691951668c");
                VRConnectionManager vrConnectionManager = new VRConnectionManager();
                VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(vrConnectionId);
                var telesRestConnection = vrConnection.Settings.CastWithValidate<TelesRestConnection>("telesRestConnection", vrConnectionId);
                if (!telesRestConnection.DefaultDomainId.HasValue)
                    throw new NullReferenceException("telesRestConnection.DefaultDomainId");

                return new AccountEnterpriseDIDDetail
                {
                    AccountName = entity.AccountId.HasValue ? new AccountBEManager().GetAccountName(accountBEDefinitionId, entity.AccountId.Value) : null,
                    ScreenNumber = entity.ScreenNumber,
                    EnterpriseName = entity.EnterpriseName,
                    EnterpriseDescription = entity.EnterpriseDescription,
                    SiteName = entity.SiteName,
                    SiteDescription = entity.SiteDescription,
                    MaxCalls = entity.MaxCalls,
                    Type = Utilities.GetEnumDescription(entity.Type)
                };
            }
            protected override Vanrise.Entities.BigResult<AccountEnterpriseDIDDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<AccountEnterpriseDIDQuery> input, IEnumerable<AccountEnterpriseDID> allRecords)
            {
                return allRecords.ToBigResult(input, null, EntityDetailMapper);
            }
            public override IEnumerable<AccountEnterpriseDID> RetrieveAllData(DataRetrievalInput<AccountEnterpriseDIDQuery> input)
            {
                Func<AccountEnterpriseDID, bool> filterExpression = (accountEnterpriseDID) =>
                {
                    if (input.Query.DIDNumber != null && !accountEnterpriseDID.ScreenNumber.Contains(input.Query.DIDNumber))
                        return false;
                    if (input.Query.AccountIds != null && (!accountEnterpriseDID.AccountId.HasValue || !input.Query.AccountIds.Contains(accountEnterpriseDID.AccountId.Value)))
                        return false;
                    return true;
                };
                return new TelesEnterpriseManager().GetAccountEnterprisesDIDs().FindAllRecords(filterExpression);
            }
            protected override ResultProcessingHandler<AccountEnterpriseDIDDetail> GetResultProcessingHandler(DataRetrievalInput<AccountEnterpriseDIDQuery> input, BigResult<AccountEnterpriseDIDDetail> bigResult)
            {
                return new ResultProcessingHandler<AccountEnterpriseDIDDetail>
                {
                    ExportExcelHandler = new AccountEnterpriseDIDExcelExportHandler(input.Query)
                };
            }
        }
        private class AccountEnterpriseDIDExcelExportHandler : ExcelExportHandler<AccountEnterpriseDIDDetail>
        {
            AccountEnterpriseDIDQuery _query;
            public AccountEnterpriseDIDExcelExportHandler(AccountEnterpriseDIDQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<AccountEnterpriseDIDDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Enterprises",
                    Header = new ExportExcelHeader
                    {
                        Cells = new List<ExportExcelHeaderCell> {
                            new ExportExcelHeaderCell { Title = "Account Name" },
                            new ExportExcelHeaderCell { Title = "Enterprise Name" },
                            new ExportExcelHeaderCell { Title = "Enterprise Description" },
                            new ExportExcelHeaderCell { Title = "Site Name" },
                            new ExportExcelHeaderCell { Title = "Site Description" },
                            new ExportExcelHeaderCell { Title = "Screen Number" },
                            new ExportExcelHeaderCell { Title = "Type" },
                            new ExportExcelHeaderCell { Title = "Channels" }
                        }
                    }
                };

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var results = context.BigResult as BigResult<AccountEnterpriseDIDDetail>;

                    if (results != null && results.Data != null)
                    {
                        sheet.Rows = new List<ExportExcelRow>();
                        foreach (var item in results.Data)
                        {
                            var accountNameRow = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            accountNameRow.Cells.Add(new ExportExcelCell { Value = item.AccountName });
                            accountNameRow.Cells.Add(new ExportExcelCell { Value = item.EnterpriseName });
                            accountNameRow.Cells.Add(new ExportExcelCell { Value = item.EnterpriseDescription });
                            accountNameRow.Cells.Add(new ExportExcelCell { Value = item.SiteName });
                            accountNameRow.Cells.Add(new ExportExcelCell { Value = item.SiteDescription });
                            accountNameRow.Cells.Add(new ExportExcelCell { Value = item.ScreenNumber });
                            accountNameRow.Cells.Add(new ExportExcelCell { Value = item.Type });
                            accountNameRow.Cells.Add(new ExportExcelCell { Value = item.MaxCalls });
                            sheet.Rows.Add(accountNameRow);
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion

        #region Private Methods
        private struct GetEnterpriseCacheName
        {
            public Guid VRConnectionId { get; set; }
            public string TelesDomainId { get; set; }
        }
        private Dictionary<string, TelesEnterpriseInfo> GetCachedEnterprises(Guid vrConnectionId, string telesDomainId, bool handleTelesNotAvailable)
        {
            CachedEnterprisesInfo enterpriseInfos = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(new GetEnterpriseCacheName
            {
                TelesDomainId = telesDomainId,
                VRConnectionId = vrConnectionId
            },
               () =>
               {
                   try
                   {
                       TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
                       string actionPath = null;
                       if (telesDomainId != null)
                       {
                           actionPath = string.Format("/domain/{0}/sub", telesDomainId);
                       }
                       else
                       {
                           actionPath = string.Format("/domain/{0}/sub", telesRestConnection.DefaultDomainId.Value);
                       }
                       List<dynamic> enterprises = telesRestConnection.Get<List<dynamic>>(actionPath);
                       List<TelesEnterpriseInfo> telesEnterpriseInfo = new List<TelesEnterpriseInfo>();
                       if (enterprises != null)
                       {
                           foreach (var enterprise in enterprises)
                           {
                               telesEnterpriseInfo.Add(new TelesEnterpriseInfo
                               {
                                   Name = enterprise.name,
                                   TelesEnterpriseId = enterprise.id.Value.ToString(),
                                   EnterpriseType = enterprise.type == "RESIDENTIAL" ? EnterpriseType.Residential : EnterpriseType.Enterprise
                               });
                           }
                       }
                       return new CachedEnterprisesInfo
                       {
                           EnterpriseInfos = telesEnterpriseInfo.ToDictionary(x => x.TelesEnterpriseId, x => x),
                           IsValid = true
                       };
                   }
                   catch (Exception ex)//handle the case where Teles API is not available
                   {
                       LoggerFactory.GetExceptionLogger().WriteException(ex);
                       return new CachedEnterprisesInfo
                       {
                           IsValid = false
                       };
                   }
               });
            if (enterpriseInfos.IsValid)
            {
                return enterpriseInfos.EnterpriseInfos;
            }
            else
            {
                if (handleTelesNotAvailable)
                    return null;
                else
                    throw new VRBusinessException("Cannot connect to Teles API");
            }
        }

        private TelesRestConnection GetTelesRestConnection(Guid vrConnectionId)
        {
            VRConnectionManager vrConnectionManager = new VRConnectionManager();
            VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(vrConnectionId);
            return vrConnection.Settings.CastWithValidate<TelesRestConnection>("telesRestConnection", vrConnectionId);
        }
        private List<dynamic> GetScreenNumbersByCountryCode(Guid vrConnectionId, string countryCode)
        {
            var telesRestConnection = GetTelesRestConnection(vrConnectionId);
            string actionPath = string.Format("/screenNum/search?cc={0}&limit=1000", countryCode);
            return telesRestConnection.Get<List<dynamic>>(actionPath);
        }
        private List<dynamic> GetUsersBTs(Guid vrConnectionId)
        {
            var telesRestConnection = GetTelesRestConnection(vrConnectionId);
            string actionPath = string.Format("/user/search?role=BT");
            return telesRestConnection.Get<List<dynamic>>(actionPath);
        }
        private List<dynamic> GetDomainLists(Guid vrConnectionId)
        {
            var telesRestConnection = GetTelesRestConnection(vrConnectionId);
            string actionPath = string.Format("/domain/list");
            return telesRestConnection.Get<List<dynamic>>(actionPath);
        }
        private List<dynamic> GetUserBTReports(Guid vrConnectionId)
        {
            var telesRestConnection = GetTelesRestConnection(vrConnectionId);
            string actionPath = string.Format("/user/businessTrunkReport");
            return telesRestConnection.Get<List<dynamic>>(actionPath);
        }
        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings.CastWithValidate<TelesEnterpriseBEDefinitionSettings>("context.EntityDefinition.Settings");

            var cachedEnterprises = GetCachedEnterprises(telesBEDefinitionSettings.VRConnectionId, null, false);

            return cachedEnterprises.Values.Select(itm => itm as dynamic).ToList();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings as TelesEnterpriseBEDefinitionSettings;
            return GetEnterprise(telesBEDefinitionSettings.VRConnectionId, null, context.EntityId);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            var telesBEDefinitionSettings = context.EntityDefinition.Settings as TelesEnterpriseBEDefinitionSettings;
            return GetEnterpriseName(telesBEDefinitionSettings.VRConnectionId, context.EntityId.ToString(), null);
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var telesEnterpriseInfo = context.Entity as TelesEnterpriseInfo;
            return telesEnterpriseInfo.TelesEnterpriseId;
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
}
