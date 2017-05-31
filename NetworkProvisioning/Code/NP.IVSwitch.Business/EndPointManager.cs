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

        public string GetEndPointDescription(EndPoint endPoint)
        {
            switch(endPoint.EndPointType)
            {
                case UserType.ACL: return String.Format("ACL - {0}", endPoint.Host);
                case UserType.SIP: return String.Format("SIP - {0}", endPoint.SipLogin);
                case UserType.VendroTermRoute: return string.Format("VendorTermRoute - {0}", endPoint.Description);
                default: throw new NotSupportedException(endPoint.EndPointType.ToString());
            }
        }

        public string GetEndPointDescription(int endPointId)
        {
            var endPoint = GetEndPoint(endPointId);
            return endPoint != null ? GetEndPointDescription(endPoint) : null;
        }

        public IEnumerable<EndPointEntityInfo> GetEndPointsInfo(EndPointInfoFilter filter)
        {
            HashSet<int> assignEndPointIds = null;
            Func<EndPoint, bool> filterFunc = null;
            int? carrierAccountSWCustomerAccountId = null;
            var allEndPoints = this.GetCachedEndPoint();
            if (filter != null)
            {
                if (filter.AssignableToCarrierAccountId.HasValue)
                {
                    assignEndPointIds = new HashSet<int>(GetCarrierAccountIdsByEndPointId().Keys);
                    carrierAccountSWCustomerAccountId = new AccountManager().GetCarrierAccountSWCustomerAccountId(filter.AssignableToCarrierAccountId.Value);
                }
                filterFunc = (x) =>
                {
                    if (filter.AssignableToCarrierAccountId.HasValue)
                    {
                        if (assignEndPointIds.Contains(x.EndPointId))
                            return false;
                        if (carrierAccountSWCustomerAccountId.HasValue && x.AccountId != carrierAccountSWCustomerAccountId.Value)
                            return false;
                    }
                    return true;
                };
            }          
            return allEndPoints.MapRecords(EndPointEntityInfoMapper, filterFunc);
        }

        public List<int> GetCarrierAccountEndPointIds(CarrierAccount carrierAccount)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            EndPointCarrierAccountExtension endPointsExtendedSettings =
                carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(carrierAccount);
            if (endPointsExtendedSettings == null) return null;
            List<int> endPointIds = new List<int>();
            if (endPointsExtendedSettings.AclEndPointInfo != null)
                endPointIds.AddRange(endPointsExtendedSettings.AclEndPointInfo.Select(itm => itm.EndPointId));
            if (endPointsExtendedSettings.UserEndPointInfo != null)
                endPointIds.AddRange(endPointsExtendedSettings.UserEndPointInfo.Select(itm => itm.EndPointId));
            return endPointIds;
        }

        public List<int> GetCarrierAccountEndPointIds(int carrierAccountId)
        {
            var carrierAccount = new CarrierAccountManager().GetCarrierAccount(carrierAccountId);
            return carrierAccount != null ? GetCarrierAccountEndPointIds(carrierAccount) : null;
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
                IpAddressHelper helper = new IpAddressHelper();
                string message;
                if (helper.ValidateSameAccountHost(hosts, endPointItem.Entity, out message))
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

        public int GetEndPointCarrierAccountId(int endPointId)
        {
            return GetCarrierAccountIdsByEndPointId().GetRecord(endPointId);
        }

        public void LinkCarrierAccountToEndPoints(int carrierAccountId, List<int> endPointIds)
        {
            if (endPointIds == null || endPointIds.Count == 0)
                throw new ArgumentNullException("endPointIds");
            List<EndPoint> endPoints = new List<EndPoint>();
            int? customerAccountId = null;
            foreach(var endPointId in endPointIds)
            {                
                var endPoint = GetEndPoint(endPointId);
                endPoint.ThrowIfNull("endPoint", endPointId);
                endPoints.Add(endPoint);
                if (!customerAccountId.HasValue)
                    customerAccountId = endPoint.AccountId;
                else if (customerAccountId.Value != endPoint.AccountId)
                    throw new Exception("All endpoints should have same AccountId");
            }
            new AccountManager().TrySetSWCustomerAccountId(carrierAccountId, customerAccountId.Value);
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            EndPointCarrierAccountExtension endPointCarrierAccountExtension = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(carrierAccountId);
            if (endPointCarrierAccountExtension == null)
                endPointCarrierAccountExtension = new EndPointCarrierAccountExtension();
            foreach(var endPoint in endPoints)
            {
                switch(endPoint.EndPointType)
                {
                    case UserType.ACL:
                        {
                            if (endPointCarrierAccountExtension.AclEndPointInfo == null)
                                endPointCarrierAccountExtension.AclEndPointInfo = new List<EndPointInfo>();
                            endPointCarrierAccountExtension.AclEndPointInfo.Add(new EndPointInfo { EndPointId = endPoint.EndPointId });
                        }break;
                    case UserType.SIP:
                        {
                            if (endPointCarrierAccountExtension.UserEndPointInfo == null)
                                endPointCarrierAccountExtension.UserEndPointInfo = new List<EndPointInfo>();
                            endPointCarrierAccountExtension.UserEndPointInfo.Add(new EndPointInfo { EndPointId = endPoint.EndPointId });
                        }
                        break;
                    default: throw new NotSupportedException(String.Format("endPoint.EndPointType '{0}'", endPoint.EndPointType));
                }
            }

            carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointCarrierAccountExtension>(
                carrierAccountId, endPointCarrierAccountExtension);
            string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierAccountId);
            foreach (var endPointId in endPointIds)
            {
                GenerateRule(carrierAccountId, endPointId, carrierAccountName);
            }
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
            var ids = GetCarrierAccountIdsByEndPointId();
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
            IpAddressHelper helper = new IpAddressHelper();
            Dictionary<int, Entities.EndPoint> hosts = GetCachedEndPoint();
            if (accountExtended != null && accountExtended.CustomerAccountId.HasValue)
            {
                endPointItem.Entity.AccountId = accountExtended.CustomerAccountId.Value;
                return helper.ValidateSameAccountHost(hosts, endPointItem.Entity, out mssg);
            }
            if (helper.IsInSameSubnet(hosts, endPointItem.Entity.Host, out mssg)) return true;
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
        
        public Dictionary<int, int> GetCarrierAccountIdsByEndPointId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOne.WhS.BusinessEntity.Business.CarrierAccountManager.CacheManager>().GetOrCreateObject("IVSwitch_GetCarrierAccountIdsByEndPointId", 
                () =>
                {
                    Dictionary<int, int> result = new Dictionary<int, int>();                   
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
                });            
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

        public EndPointEntityInfo EndPointEntityInfoMapper(EndPoint endPoint)
        {
            EndPointEntityInfo endPointEntityInfo = new EndPointEntityInfo
            {
               EndPointId = endPoint.EndPointId,
               Description = GetEndPointDescription(endPoint),
               AccountId = endPoint.AccountId
            };

            return endPointEntityInfo;
        }

        #endregion
    }
}