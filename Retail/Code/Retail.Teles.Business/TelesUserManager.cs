using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Retail.Teles.Business.AccountBEActionTypes;
namespace Retail.Teles.Business
{
    public class TelesUserManager : BaseBusinessEntityManager
    {
        #region Public Methods
        AccountBEManager _accountBEManager = new AccountBEManager();
        public string CreateUser(Guid vrConnectionId, string siteName, string gateway, User request)
        {

            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            var actionPath = string.Format("/domain/{0}/user?gateway={1}", siteName, gateway);
            VRWebAPIResponse<string> response = telesRestConnection.Post<User, string>(actionPath, request, true);
            response.Headers.Location.ThrowIfNull("response.Headers", response.Headers);
            var userId = response.Headers.Location.Segments.Last();
            userId.ThrowIfNull("userId", userId);
            TelesUserManager.SetCacheExpired();
            return Convert.ToString(userId);
        }
        public IEnumerable<TelesUserInfo> GetUsersInfo(Guid vrConnectionId, string siteId, TelesUserFilter filter)
        {
            var cachedSites = GetUsersInfoBySiteId(vrConnectionId, siteId);

            Func<TelesUserInfo, bool> filterFunc = (telesUserInfo) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null)
                    {
                        foreach (var item in filter.Filters)
                        {
                            TelesUserFilterContext context = new TelesUserFilterContext
                            {
                                AccountBEDefinitionId = filter.AccountBEDefinitionId,
                                UserId = telesUserInfo.TelesUserId
                            };
                            if (item.IsExcluded(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return cachedSites.FindAllRecords(filterFunc).OrderBy(x => x.Name);

        }
        public IEnumerable<dynamic> GetUsers(Guid vrConnectionId, string siteId)
        {
            var actionPath = string.Format("/domain/{0}/user", siteId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            List<dynamic> sites = telesRestConnection.Get<List<dynamic>>(actionPath);
            return sites;
        }
        public dynamic UpdateUser(Guid vrConnectionId, dynamic user)
        {
            var actionPath = string.Format("/user/{0}", user.id);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            return telesRestConnection.Put<dynamic, dynamic>(actionPath, user);
        }
        public dynamic GetUser(Guid vrConnectionId, string userId)
        {
            var actionPath = string.Format("/user/{0}", userId);
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);

            return telesRestConnection.Get<dynamic>(actionPath);
        }
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> MapUserToAccount(MapUserToAccountInput input)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            if (CanMapTelesUser(input.AccountBEDefinitionId, input.TelesUserId) && IsMapUserToAccountValid(input.AccountBEDefinitionId, input.AccountId, input.ActionDefinitionId))
            {
                bool result = TryMapUserToAccount(input.AccountBEDefinitionId, input.AccountId,input.TelesDomainId,input.TelesEnterpriseId ,input.TelesSiteId,input.TelesUserId);
                if (result)
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    _accountBEManager.TrackAndLogObjectCustomAction(input.AccountBEDefinitionId, input.AccountId, "Map To Teles User", null, null);
                    updateOperationOutput.UpdatedObject = _accountBEManager.GetAccountDetail(input.AccountBEDefinitionId, input.AccountId);
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
            }
            return updateOperationOutput;

        }
        public bool IsMapUserToAccountValid(Guid accountBEDefinitionId, long accountId, Guid actionDefinitionId)
        {

            var accountDefinitionAction = new AccountBEDefinitionManager().GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId);
            if (accountDefinitionAction != null)
            {
                var settings = accountDefinitionAction.ActionDefinitionSettings as MappingTelesUserActionSettings;
                if (settings != null)
                {
                    var account = _accountBEManager.GetAccount(accountBEDefinitionId, accountId);
                    return _accountBEManager.EvaluateAccountCondition(account, accountDefinitionAction.AvailabilityCondition);
                }

            }
            return false;
        }
        public bool TryMapUserToAccount(Guid accountBEDefinitionId, long accountId,string telesDomainId,string telesEnterpriseId, string telesSiteId, string telesUserId, ProvisionStatus? status = null)
        {
            UserAccountMappingInfo userAccountMappingInfo = new UserAccountMappingInfo { 
                TelesDomainId = telesDomainId,
                TelesEnterpriseId = telesEnterpriseId,
                TelesSiteId = telesSiteId,
                TelesUserId = telesUserId,
                Status = status 
            };
            return _accountBEManager.UpdateAccountExtendedSetting<UserAccountMappingInfo>(accountBEDefinitionId, accountId, userAccountMappingInfo);
        }
        public bool CanMapTelesUser(Guid accountBEDefinitionId, string userId)
        {
            var cachedAccountsByUsers = GetCachedAccountsByUsers(accountBEDefinitionId);
            if (cachedAccountsByUsers != null && cachedAccountsByUsers.ContainsKey(userId))
                return false;
            return true;
        }
        public static void SetCacheExpired()
        {
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }
        public bool DoesUserHaveExecutePermission(Guid accountBEDefinitionId)
        {
            var accountDefinitionActions = new AccountBEDefinitionManager().GetAccountActionDefinitions(accountBEDefinitionId);
            foreach (var a in accountDefinitionActions)
            {
                var settings = a.ActionDefinitionSettings as MappingTelesUserActionSettings;
                if (settings != null)
                    return settings.DoesUserHaveExecutePermission();
            }
            return false;
        }
        public string GetUsereName(Guid vrConnectionId, dynamic telesUserId) // to be removed later
        {
            return GetUserName(vrConnectionId, telesUserId);
        }
        public string GetUserName(Guid vrConnectionId, dynamic telesUserId)
        {
            var user = GetUser(vrConnectionId, telesUserId);
            if (user == null)
                return null;
            return user.name;
        }
        public string CreateNumberRanges(Guid vrConnectionId, string telesUserId, List<NumberRange> numberRangs)
        {
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            var actionPath = string.Format("/user/{0}/numRange/add", telesUserId);
            string response = telesRestConnection.Put<List<NumberRange>, string>(actionPath, numberRangs);
            return response;
        }
        public string CreateMSN(Guid vrConnectionId, string telesUserId, PhoneNumber msn)
        {
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            var actionPath = string.Format("/user/{0}/msn", telesUserId);
            string response = telesRestConnection.Post<PhoneNumber, string>(actionPath, msn);
            return response;
        }

        public string CreatePhoneNumbers(Guid vrConnectionId, string telesUserId, List<PhoneNumber> phoneNumbers)
        {
            TelesRestConnection telesRestConnection = GetTelesRestConnection(vrConnectionId);
            var actionPath = string.Format("/user/{0}/phoneNumList", telesUserId);
            string response = telesRestConnection.Post<List<PhoneNumber>, string>(actionPath, phoneNumbers);
            return response;
        }

        public int GetAccountDIDsCount(Guid accountBEDefinitionId, long accountId)
        {
            DIDManager didManager = new DIDManager();
            var dids = didManager.GetDIDsByParentId(accountId.ToString(), DateTime.Now);
            List<string> allDIDs = new List<string>();
            if (dids != null)
            {
                foreach (var did in dids)
                {
                    List<string> didNumbers = didManager.GetAllDIDNumbers(did);
                    if (didNumbers != null)
                    {
                        allDIDs.AddRange(didNumbers);
                    }
                }
            }
            return allDIDs.Count;
        }
        public Dictionary<string, long> GetCachedAccountsByUsers(Guid accountBEDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>().GetOrCreateObject("GetCachedAccountsByUsers", accountBEDefinitionId, () =>
            {
                var accountBEManager = new AccountBEManager();
                Dictionary<string, long> accountsByUser = null;
                var cashedAccounts = accountBEManager.GetAccounts(accountBEDefinitionId);
                foreach (var item in cashedAccounts)
                {
                    var userAccountMappingInfo = accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(item.Value);
                    if (userAccountMappingInfo != null)
                    {
                        if (accountsByUser == null)
                            accountsByUser = new Dictionary<string, long>();
                        accountsByUser.Add(userAccountMappingInfo.TelesUserId, item.Key);
                    }
                }
                return accountsByUser;
            });
        }

        public TelesUserMappingInfo GetUserTelesInfo(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId)
        {
            var userAccountInfo = _accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(accountBEDefinitionId, accountId);
            if (userAccountInfo != null)
            {
                TelesUserMappingInfo telesUserMappingInfo = new TelesUserMappingInfo();
                if (userAccountInfo.TelesUserId != null)
                {
                    TelesUserManager telesUserManager = new Business.TelesUserManager();
                    var user = telesUserManager.GetUser(vrConnectionId, userAccountInfo.TelesUserId);
                    if (user != null)
                    {
                        telesUserMappingInfo.FirstName = user.firstName;
                        telesUserMappingInfo.LastName = user.lastName;
                        telesUserMappingInfo.LoginName = user.loginName;
                        TelesSiteManager telesSiteManager = new TelesSiteManager();
                        telesUserMappingInfo.EnterpriseName = new TelesEnterpriseManager().GetEnterpriseName(vrConnectionId, userAccountInfo.TelesEnterpriseId.ToString(), user.serviceProviderId.ToString());
                        telesUserMappingInfo.SiteName = telesSiteManager.GetSiteName(vrConnectionId, user.domainId);

                        var siteRoutinrGroups = telesSiteManager.GetSiteRoutingGroups(vrConnectionId, user.domainId);
                        if (siteRoutinrGroups != null)
                        {
                            foreach(var siteRoutinrGroup in siteRoutinrGroups)
                            {
                                if(siteRoutinrGroup.Value.id == user.routingGroupId)
                                {
                                    telesUserMappingInfo.RoutingGroupName = siteRoutinrGroup.Value.name;
                                    break;
                                }
                            }
                         
                        }
                    }
                    return telesUserMappingInfo;
                }
            }
            return null;
        }
        public string GetUserTelesSiteId(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId)
        {
            var userAccountInfo = _accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(accountBEDefinitionId, accountId);
            if (userAccountInfo != null)
            {
                TelesUserMappingInfo telesUserMappingInfo = new TelesUserMappingInfo();
                if (userAccountInfo.TelesUserId != null)
                {
                    TelesUserManager telesUserManager = new Business.TelesUserManager();
                    var user = telesUserManager.GetUser(vrConnectionId, userAccountInfo.TelesUserId);
                    if (user != null)
                    {
                        return user.domainId;
                    }
                }
            }
            return null;
        }
        public string GetCurrentUserRoutingGroupId(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId)
        {
            var userAccountInfo = _accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(accountBEDefinitionId, accountId);
            if (userAccountInfo != null)
            {
                TelesUserMappingInfo telesUserMappingInfo = new TelesUserMappingInfo();
                if (userAccountInfo.TelesUserId != null)
                {
                    TelesUserManager telesUserManager = new Business.TelesUserManager();
                    var user = telesUserManager.GetUser(vrConnectionId, userAccountInfo.TelesUserId);
                    if (user != null)
                    {
                        return user.routingGroupId.ToString();
                    }
                }
            }
            return null;
        }
        public UpdateOperationOutput<AccountDetail> ChangeUserRoutingGroup(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId, string routingGroupId)
        {
            UpdateOperationOutput<AccountDetail> updateOperationOutput = new UpdateOperationOutput<AccountDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            var userAccountInfo = _accountBEManager.GetExtendedSettings<UserAccountMappingInfo>(accountBEDefinitionId, accountId);
            if (userAccountInfo != null)
            {
                TelesUserMappingInfo telesUserMappingInfo = new TelesUserMappingInfo();
                if (userAccountInfo.TelesUserId != null)
                {
                    TelesUserManager telesUserManager = new Business.TelesUserManager();
                    var user = telesUserManager.GetUser(vrConnectionId, userAccountInfo.TelesUserId);
                    if (user != null)
                    {
                        if(user.routingGroupId == routingGroupId)
                        {
                            updateOperationOutput.Result = UpdateOperationResult.SameExists;
                        }else
                        {
                            var currentRoutingGroupId = user.routingGroupId;
                            
                            user.routingGroupId = routingGroupId;
                            telesUserManager.UpdateUser(vrConnectionId, user);
                            TelesSiteManager telesSiteManager = new TelesSiteManager();
                            var routingGroups = telesSiteManager.GetSiteRoutingGroups(vrConnectionId, user.domainId);
                            string newRoutingGroupName = null;
                            string currentRoutingGroupName = null;
                            if (routingGroups != null)
                            {
                                foreach( var routingGroup in routingGroups)
                                {
                                    if(routingGroup.Value.id == routingGroupId)
                                    {
                                        newRoutingGroupName = routingGroup.Value.name;
                                    }
                                    else if (routingGroup.Value.id == currentRoutingGroupId)
                                    {
                                        currentRoutingGroupName = routingGroup.Value.name;
                                    }
                                }
                            }
                            string customAction = newRoutingGroupName != null && currentRoutingGroupName != null ? string.Format("User routing group changed from '{0}' to '{1}'", currentRoutingGroupName, newRoutingGroupName) : null;
                            _accountBEManager.TrackAndLogObjectCustomAction(accountBEDefinitionId, accountId, "Change User Routing Group", customAction, null);
                            updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                            updateOperationOutput.UpdatedObject = _accountBEManager.GetAccountDetail(accountBEDefinitionId,accountId);
                        }
                        
                    }
                }
            }
            return updateOperationOutput;
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
        
        #endregion

        #region Private Methods
        private Dictionary<string, TelesUserInfo> GetUsersInfoBySiteId(Guid vrConnectionId, string siteId)
        {
            var users = GetUsers(vrConnectionId, siteId);
            List<TelesUserInfo> telesUsersInfo = new List<TelesUserInfo>();
            if (users != null)
            {
                foreach (var user in users)
                {
                    telesUsersInfo.Add(new TelesUserInfo
                    {
                        Name = user.loginName,
                        TelesUserId = user.id.Value.ToString()
                    });
                }
            }
            return telesUsersInfo.ToDictionary(x => x.TelesUserId, x => x);
        }

        private TelesRestConnection GetTelesRestConnection(Guid vrConnectionId)
        {
            VRConnectionManager vrConnectionManager = new VRConnectionManager();
            VRConnection vrConnection = vrConnectionManager.GetVRConnection<TelesRestConnection>(vrConnectionId);
            return vrConnection.Settings.CastWithValidate<TelesRestConnection>("telesRestConnection", vrConnectionId);
        }
        #endregion

        #region IBusinessEntityManager
        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }
        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }
        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            throw new NotImplementedException();
        }
        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
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
