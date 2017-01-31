using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using NP.IVSwitch.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Business;

namespace NP.IVSwitch.Business
{
    public class EndPointManager
    {
        #region Public Methods
        public EndPoint GetEndPoint(int endPointId)
        {
            Dictionary<int, EndPoint> cachedEndPoint = this.GetCachedEndPoint();
            return cachedEndPoint.GetRecord(endPointId);
        }
        public IDataRetrievalResult<EndPointDetail> GetFilteredEndPoints(DataRetrievalInput<EndPointQuery> input)
        {
            //Get Carrier by id
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            EndPointCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(input.Query.CarrierAccountId.Value);

            Dictionary<int, EndPointInfo> aclEndPointDic = new Dictionary<int, EndPointInfo>();
            Dictionary<int, EndPointInfo> userEendPointDic = new Dictionary<int, EndPointInfo>();

            if (extendedSettingsObject != null)
            {
                if (extendedSettingsObject.AclEndPointInfo != null) aclEndPointDic = extendedSettingsObject.AclEndPointInfo.ToDictionary(k => k.EndPointId, v => v);
                if (extendedSettingsObject.UserEndPointInfo != null) userEendPointDic = extendedSettingsObject.UserEndPointInfo.ToDictionary(k => k.EndPointId, v => v);
            }
            var allEndPoints = GetCachedEndPoint();
            Func<EndPoint, bool> filterExpression =
                x => (aclEndPointDic.ContainsKey(x.EndPointId) || (userEendPointDic.ContainsKey(x.EndPointId)));

            return DataRetrievalManager.Instance.ProcessResult(input, allEndPoints.ToBigResult(input, filterExpression, EndPointDetailMapper));
        }
        public List<EndPointInfo> GetAclList(int carrierId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            EndPointCarrierAccountExtension endPointsExtendedSettings =
                carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(carrierId);
            if (endPointsExtendedSettings == null) return null;
            return endPointsExtendedSettings.AclEndPointInfo;
        }
        public InsertOperationOutput<EndPointDetail> AddEndPoint(EndPointToAdd endPointItem)
        {
            InsertOperationOutput<EndPointDetail> insertOperationOutput = new InsertOperationOutput<EndPointDetail>
             {
                 Result = InsertOperationResult.Failed,
                 InsertedObject = null
             };
            string mssg;
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            string carrierAccountName = carrierAccountManager.GetCarrierAccountName(endPointItem.CarrierAccountId);
            var profileId = carrierAccountManager.GetCarrierProfileId(endPointItem.CarrierAccountId);
            if (!profileId.HasValue) return insertOperationOutput;
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            AccountCarrierProfileExtension accountExtended =
                carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profileId.Value);
            return endPointItem.Entity.EndPointType == UserType.ACL
                ? InsertAcl(accountExtended, endPointItem, profileId.Value, carrierAccountName, out mssg)
                : InsertSip(accountExtended, endPointItem, profileId.Value, carrierAccountName);
        }
        public UpdateOperationOutput<EndPointDetail> UpdateEndPoint(EndPointToAdd endPointItem)
        {
            var updateOperationOutput = new UpdateOperationOutput<EndPointDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            if (endPointItem.Entity.EndPointType == UserType.ACL)
            {
                var hosts = GetCachedEndPoint();
                var relatedAccount = hosts.Values.FirstOrDefault(r => r.EndPointId == endPointItem.Entity.EndPointId);
                if (relatedAccount != null) endPointItem.Entity.AccountId = relatedAccount.AccountId;
                string message;
                if (IpAddressHelper.ValidateSameAccountHost(hosts, endPointItem.Entity, out message))
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        updateOperationOutput.ShowExactMessage = true;
                        updateOperationOutput.Message = message;
                    }
                    else
                        updateOperationOutput.Result = UpdateOperationResult.SameExists;
                    return updateOperationOutput;
                }
            }
            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            Helper.SetSwitchConfig(dataManager);

            if (dataManager.Update(endPointItem.Entity))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                EndPoint updatedEndPoint = GetEndPoint(endPointItem.Entity.EndPointId);
                updateOperationOutput.UpdatedObject = EndPointDetailMapper(updatedEndPoint);
                AccountManager accountManager = new AccountManager();
                accountManager.UpdateChannelLimit(updatedEndPoint.AccountId);
            }
            else
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            GetValue(endPointItem);
            return updateOperationOutput;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IEndPointDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods
        private void GetValue(EndPointToAdd endPointItem)
        {
            var ids = GetCachedIds();
            int carrierId;
            if (ids.TryGetValue(endPointItem.Entity.EndPointId, out carrierId))
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierId);
                GenerateRule(carrierId, endPointItem.Entity.EndPointId, carrierAccountName);
            }
        }
        private InsertOperationOutput<EndPointDetail> InsertAcl(AccountCarrierProfileExtension accountExtended,
        EndPointToAdd endPointItem, int profileId
        , string carrierAccountName, out string mssg)
        {
            InsertOperationOutput<EndPointDetail> insertOperationOutput = new InsertOperationOutput<EndPointDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            int endPointId;
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            List<EndPointInfo> userEndPointInfoList = new List<EndPointInfo>();
            List<EndPointInfo> aclEndPointInfoList = new List<EndPointInfo>();
            if (Validatingsubnet(accountExtended, endPointItem, profileId, out mssg))
            {
                insertOperationOutput.ShowExactMessage = true;
                insertOperationOutput.Message = mssg;
                return insertOperationOutput;
            }
            var endPointsExtendedSettings = EndPointCarrierAccountExtension(endPointItem, carrierAccountManager,
                ref aclEndPointInfoList, ref userEndPointInfoList);

            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            Helper.SetSwitchConfig(dataManager);

            if (
                !dataManager.Insert(endPointItem.Entity, userEndPointInfoList, aclEndPointInfoList, out endPointId,
                    carrierAccountName))
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
                return insertOperationOutput;
            }
            AccountManager accountManager = new AccountManager();
            accountManager.UpdateChannelLimit(endPointItem.Entity.AccountId);
            EndPointInfo endPointInfo = new EndPointInfo { EndPointId = endPointId };

            if (endPointsExtendedSettings.AclEndPointInfo == null)
                endPointsExtendedSettings.AclEndPointInfo = new List<EndPointInfo>();
            endPointsExtendedSettings.AclEndPointInfo.Add(endPointInfo);

            carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointCarrierAccountExtension>(
                endPointItem.CarrierAccountId, endPointsExtendedSettings);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            insertOperationOutput.Result = InsertOperationResult.Succeeded;
            insertOperationOutput.InsertedObject = EndPointDetailMapper(GetEndPoint(endPointId));
            GenerateRule(endPointItem.CarrierAccountId, endPointId, carrierAccountName);
            return insertOperationOutput;
        }
        private InsertOperationOutput<EndPointDetail> InsertSip(AccountCarrierProfileExtension accountExtended, EndPointToAdd endPointItem, int profileId, string carrierAccountName)
        {
            int endPointId;
            InsertOperationOutput<EndPointDetail> insertOperationOutput = new InsertOperationOutput<EndPointDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            List<EndPointInfo> userEndPointInfoList = new List<EndPointInfo>();
            List<EndPointInfo> aclEndPointInfoList = new List<EndPointInfo>();
            if (accountExtended != null && accountExtended.CustomerAccountId.HasValue)
                endPointItem.Entity.AccountId = accountExtended.CustomerAccountId.Value;
            else
                endPointItem.Entity.AccountId = CreateNewAccount(profileId);
            var endPointsExtendedSettings = EndPointCarrierAccountExtension(endPointItem, carrierAccountManager,
                ref aclEndPointInfoList, ref userEndPointInfoList);

            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            Helper.SetSwitchConfig(dataManager);

            bool succInsert = dataManager.Insert(endPointItem.Entity, userEndPointInfoList, aclEndPointInfoList, out endPointId, carrierAccountName);
            if (!succInsert) return insertOperationOutput;

            AccountManager accountManager = new AccountManager();
            accountManager.UpdateChannelLimit(endPointItem.Entity.AccountId);
            EndPointInfo endPointInfo = new EndPointInfo { EndPointId = endPointId };

            if (endPointsExtendedSettings.UserEndPointInfo == null)
                endPointsExtendedSettings.UserEndPointInfo = new List<EndPointInfo>();
            endPointsExtendedSettings.UserEndPointInfo.Add(endPointInfo);
            carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointCarrierAccountExtension>(
                endPointItem.CarrierAccountId, endPointsExtendedSettings);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            insertOperationOutput.Result = InsertOperationResult.Succeeded;
            insertOperationOutput.InsertedObject = EndPointDetailMapper(GetEndPoint(endPointId));
            GenerateRule(endPointItem.CarrierAccountId, endPointId, carrierAccountName);
            return insertOperationOutput;
        }
        private static EndPointCarrierAccountExtension EndPointCarrierAccountExtension(EndPointToAdd endPointItem,
            CarrierAccountManager carrierAccountManager, ref List<EndPointInfo> aclEndPointInfoList, ref List<EndPointInfo> userEndPointInfoList)
        {
            EndPointCarrierAccountExtension endPointsExtendedSettings =
                carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(
                    endPointItem.CarrierAccountId) ??
                new EndPointCarrierAccountExtension();

            if (endPointsExtendedSettings.AclEndPointInfo != null)
                aclEndPointInfoList = endPointsExtendedSettings.AclEndPointInfo;
            if (endPointsExtendedSettings.UserEndPointInfo != null)
                userEndPointInfoList = endPointsExtendedSettings.UserEndPointInfo;
            return endPointsExtendedSettings;
        }

        private int CreateNewAccount(int profileId)
        {
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile(profileId);
            AccountManager accountManager = new AccountManager();
            Account account = accountManager.GetAccountInfoFromProfile(carrierProfile, true);
            int accountId = accountManager.AddAccount(account);

            AccountCarrierProfileExtension extendedSettings =
                carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(profileId) ??
                new AccountCarrierProfileExtension();
            extendedSettings.CustomerAccountId = accountId;

            carrierProfileManager.UpdateCarrierProfileExtendedSetting<AccountCarrierProfileExtension>(profileId, extendedSettings);
            return accountId;
        }
        private bool Validatingsubnet(AccountCarrierProfileExtension accountExtended, EndPointToAdd endPointItem, int profileId,
           out string mssg)
        {
            Dictionary<int, Entities.EndPoint> hosts = GetCachedEndPoint();
            if (accountExtended != null && accountExtended.CustomerAccountId.HasValue)
            {
                endPointItem.Entity.AccountId = accountExtended.CustomerAccountId.Value;
                return IpAddressHelper.ValidateSameAccountHost(hosts, endPointItem.Entity, out mssg);
            }
            if (IpAddressHelper.IsInSameSubnet(hosts, endPointItem.Entity.Host, out mssg)) return true;
            endPointItem.Entity.AccountId = CreateNewAccount(profileId);
            return false;
        }

        private void GenerateRule(int carrierId, int endPointId, string carrierName)
        {
            MappingRuleHelper mappingRuleHelper = new MappingRuleHelper(carrierId, endPointId, 1, carrierName);
            mappingRuleHelper.BuildRule();
        }
        Dictionary<int, EndPoint> GetCachedEndPoint()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetEndPoint",
                () =>
                {
                    IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
                    Helper.SetSwitchConfig(dataManager);
                    return dataManager.GetEndPoints().ToDictionary(x => x.EndPointId, x => x);
                });
        }
        Dictionary<int, int> GetCachedIds()
        {
            return
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                    .GetOrCreateObject("GetEndPointWithCarrierId",
                        GetEndPointWithCarrierId);
        }

        public Dictionary<int, int> GetEndPointWithCarrierId()
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            IRouteDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteDataManager>();
            Helper.SetSwitchConfig(dataManager);
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var customers = carrierAccountManager.GetAllCustomers();
            foreach (var customerItem in customers)
            {
                EndPointCarrierAccountExtension extendedSettingsObject =
                    carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(customerItem);
                if (extendedSettingsObject == null || extendedSettingsObject.AclEndPointInfo == null) continue;
                foreach (var aclInfo in extendedSettingsObject.AclEndPointInfo)
                {
                    if (!result.ContainsKey(aclInfo.EndPointId))
                        result[aclInfo.EndPointId] = customerItem.CarrierAccountId;
                }
            }
            return result;
        }

        #endregion

        #region Mappers

        public EndPointDetail EndPointDetailMapper(EndPoint endPoint)
        {
            EndPointDetail endPointDetail = new EndPointDetail
            {
                Entity = endPoint,
                CurrentStateDescription = Utilities.GetEnumDescription<State>(endPoint.CurrentState),
                CurrentEndPointType = Utilities.GetEnumDescription<UserType>(endPoint.EndPointType)
            };

            return endPointDetail;
        }

        #endregion
    }
}