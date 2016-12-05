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

            Dictionary<int, EndPointInfo> endPointDic = new Dictionary<int, EndPointInfo>();

            if (extendedSettingsObject != null)
            {
                endPointDic = extendedSettingsObject.EndPointInfo.ToDictionary(k => k.EndPointId, v => v);
            }

            var allEndPoints = this.GetCachedEndPoint();
            Func<EndPoint, bool> filterExpression = (x) => (endPointDic.ContainsKey(x.EndPointId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allEndPoints.ToBigResult(input, filterExpression, EndPointDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<EndPointDetail> AddEndPoint(EndPointToAdd endPointItem)
        {
            int carrierAccountId = endPointItem.CarrierAccountId;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId);


            int carrierProfileId = (int)carrierAccountManager.GetCarrierProfileId(carrierAccountId); // Get CarrierProfileId

            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId); // Get CarrierProfile

            AccountCarrierProfileExtension accountExtended = carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(carrierProfileId);
            int accountId = -1;


            if (accountExtended != null && accountExtended.CustomerAccountId.HasValue)
            {
                accountId = accountExtended.CustomerAccountId.Value;
            }
            else
            {
                //create the account
                AccountManager accountManager = new AccountManager();
                Account account = accountManager.GetAccountInfoFromProfile(carrierProfile, true);
                accountId = accountManager.AddAccount(account);

                // add it to extendedsettings
                AccountCarrierProfileExtension extendedSettings = new AccountCarrierProfileExtension();
                AccountCarrierProfileExtension ExtendedSettingsObject = carrierProfileManager.GetExtendedSettings<AccountCarrierProfileExtension>(carrierProfileId);
                if (ExtendedSettingsObject != null)
                    extendedSettings = (AccountCarrierProfileExtension)ExtendedSettingsObject;

                extendedSettings.CustomerAccountId = accountId;

                carrierProfileManager.UpdateCarrierProfileExtendedSetting<AccountCarrierProfileExtension>(carrierProfileId, extendedSettings);



            }

            endPointItem.Entity.AccountId = accountId;

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<EndPointDetail>
            {
                Result = Vanrise.Entities.InsertOperationResult.Failed,
                InsertedObject = null
            };


            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            Helper.SetSwitchConfig(dataManager);
            int endPointId;


            EndPointCarrierAccountExtension endPointsExtendedSettings = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(carrierAccountId);

            if (endPointsExtendedSettings == null)
                endPointsExtendedSettings = new EndPointCarrierAccountExtension();

            List<EndPointInfo> endPointInfoList = new List<EndPointInfo>();
            if (endPointsExtendedSettings.EndPointInfo != null)
                endPointInfoList = endPointsExtendedSettings.EndPointInfo;


            if (dataManager.Insert(endPointItem.Entity, endPointInfoList, out  endPointId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = EndPointDetailMapper(GetEndPoint(endPointId));


                // tariffs and routes
                string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierAccount.CarrierAccountId);
                dataManager.CheckTariffAndRouteTables(insertOperationOutput.InsertedObject.Entity, carrierAccountName);


                EndPointInfo endPointInfo = new EndPointInfo { EndPointId = endPointId };

                endPointInfoList.Add(endPointInfo);
                endPointsExtendedSettings.EndPointInfo = endPointInfoList;

                carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointCarrierAccountExtension>(carrierAccountId, endPointsExtendedSettings);

            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }



        public Vanrise.Entities.UpdateOperationOutput<EndPointDetail> UpdateEndPoint(EndPointToAdd endPointItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<EndPointDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();
            Helper.SetSwitchConfig(dataManager);

            if (dataManager.Update(endPointItem.Entity))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = EndPointDetailMapper(this.GetEndPoint(endPointItem.Entity.EndPointId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

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

        #endregion

        #region Mappers

        public EndPointDetail EndPointDetailMapper(EndPoint endPoint)
        {
            EndPointDetail endPointDetail = new EndPointDetail()
            {
                Entity = endPoint,
                CurrentStateDescription = Vanrise.Common.Utilities.GetEnumDescription<State>(endPoint.CurrentState),
                CurrentEndPointType = Vanrise.Common.Utilities.GetEnumDescription<EndPointType>(endPoint.EndPointType),

            };

            return endPointDetail;
        }

        #endregion
    }
}