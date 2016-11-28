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


            EndPointExtended extendedSettingsObject = carrierAccountManager.GetExtendedSettingsObject<EndPointExtended>(input.Query.CarrierAccountId.Value);

            List<int> endPointIdList = new List<int>();

            if (extendedSettingsObject != null)
            {
                endPointIdList = extendedSettingsObject.EndPointIds;
            }

             var allEndPoints = this.GetCachedEndPoint();
             Func<EndPoint, bool> filterExpression = (x) => (endPointIdList.Contains(x.EndPointId));                                                             
            
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

            AccountExtended accountExtended = carrierProfileManager.GetExtendedSettingsObject<AccountExtended>(carrierProfileId);
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
                AccountExtended extendedSettings = new AccountExtended();
                AccountExtended ExtendedSettingsObject = carrierProfileManager.GetExtendedSettingsObject<AccountExtended>(carrierProfileId);
                if (ExtendedSettingsObject != null)
                    extendedSettings = (AccountExtended)ExtendedSettingsObject;

                extendedSettings.CustomerAccountId = accountId;

                carrierProfileManager.UpdateCarrierProfileExtendedSetting<AccountExtended>(carrierProfileId, extendedSettings);

                

            }

            endPointItem.Entity.AccountId = accountId;

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<EndPointDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();

            int endPointId = -1;
 
            if (dataManager.Insert(endPointItem.Entity,out  endPointId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = EndPointDetailMapper(this.GetEndPoint(endPointId));

                EndPointExtended endPointsExtendedSettings =carrierAccountManager.GetExtendedSettingsObject<EndPointExtended>(carrierAccountId);
 
                if (endPointsExtendedSettings == null)
                    endPointsExtendedSettings = new EndPointExtended();

                List<int> endPointIds = new List<int>();
                if (endPointsExtendedSettings.EndPointIds != null)
                {
                    endPointIds = endPointsExtendedSettings.EndPointIds;
                    // add tariff and route table
                    dataManager.InsertTariff(carrierAccount.NameSuffix);

                }
                else
                {
                    // tariff and route table already exist
                    // pass route_id and tariff_id to endpoint
                }

                endPointIds.Add(endPointId);
                endPointsExtendedSettings.EndPointIds = endPointIds;

                carrierAccountManager.UpdateCarrierAccountExtendedSetting<EndPointExtended>(carrierAccountId, endPointsExtendedSettings);

            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<EndPointDetail> UpdateEndPoint(EndPointToAdd endPointItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<EndPointDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IEndPointDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IEndPointDataManager>();

 
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