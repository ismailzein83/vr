﻿using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ChargingPolicyManager
    {
        #region Fields

        ServiceTypeManager _serviceTypeManager = new ServiceTypeManager();
        
        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ChargingPolicyDetail> GetFilteredChargingPolicies(Vanrise.Entities.DataRetrievalInput<ChargingPolicyQuery> input)
        {
            Dictionary<int, ChargingPolicy> cachedChargingPolicies = this.GetCachedChargingPolicies();

            Func<ChargingPolicy, bool> filterExpression = (chargingPolicy) =>
                (input.Query.Name == null || chargingPolicy.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.ServiceTypeIds == null || input.Query.ServiceTypeIds.Contains(chargingPolicy.ServiceTypeId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedChargingPolicies.ToBigResult(input, filterExpression, ChargingPolicyDetailMapper));
        }

        public ChargingPolicy GetChargingPolicy(int chargingPolicyId)
        {
            return this.GetCachedChargingPolicies().GetRecord(chargingPolicyId);
        }

        public Vanrise.Entities.InsertOperationOutput<ChargingPolicyDetail> AddChargingPolicy(ChargingPolicy chargingPolicy)
        {
            ValidateChargingPolicyToAdd(chargingPolicy);

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ChargingPolicyDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IChargingPolicyDataManager dataManager = BEDataManagerFactory.GetDataManager<IChargingPolicyDataManager>();
            int chargingPolicyId = -1;

            if (dataManager.Insert(chargingPolicy, out chargingPolicyId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                chargingPolicy.ChargingPolicyId = chargingPolicyId;
                insertOperationOutput.InsertedObject = ChargingPolicyDetailMapper(chargingPolicy);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<ChargingPolicyDetail> UpdateChargingPolicy(ChargingPolicyToEdit chargingPolicy)
        {
            ValidateChargingPolicyToEdit(chargingPolicy);

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ChargingPolicyDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IChargingPolicyDataManager dataManager = BEDataManagerFactory.GetDataManager<IChargingPolicyDataManager>();

            if (dataManager.Update(chargingPolicy))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ChargingPolicyDetailMapper(this.GetChargingPolicy(chargingPolicy.ChargingPolicyId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Validation Methods

        private void ValidateChargingPolicyToAdd(ChargingPolicy chargingPolicy)
        {
            ValidateChargingPolicy(chargingPolicy.Name, chargingPolicy.ServiceTypeId, chargingPolicy.Settings);
        }

        private void ValidateChargingPolicyToEdit(ChargingPolicyToEdit chargingPolicy)
        {
            ChargingPolicy chargingPolicyEntity = this.GetChargingPolicy(chargingPolicy.ChargingPolicyId);
            
            if (chargingPolicyEntity == null)
                throw new DataIntegrityValidationException(String.Format("ChargingPolicy '{0}' does not exist", chargingPolicy.ChargingPolicyId));

            ValidateChargingPolicy(chargingPolicy.Name, chargingPolicyEntity.ServiceTypeId, chargingPolicy.Settings);
        }

        private void ValidateChargingPolicy(string name, int serviceTypeId, ChargingPolicySettings settings)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new MissingArgumentValidationException("ChargingPolicy.Name");

            var serviceTypeManager = new ServiceTypeManager();
            ServiceType serviceType = serviceTypeManager.GetServiceType(serviceTypeId);
            if (serviceType == null)
                throw new DataIntegrityValidationException(String.Format("ServiceType '{0}' does not exist", serviceTypeId));

            if (settings == null)
                throw new MissingArgumentValidationException("ChargingPolicy.Settings");
            if (settings.Parts == null)
                throw new MissingArgumentValidationException("ChargingPolicy.Settings.Parts");
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IChargingPolicyDataManager _dataManager = BEDataManagerFactory.GetDataManager<IChargingPolicyDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreChargingPoliciesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, ChargingPolicy> GetCachedChargingPolicies()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetChargingPolicies", () =>
            {
                IChargingPolicyDataManager dataManager = BEDataManagerFactory.GetDataManager<IChargingPolicyDataManager>();
                IEnumerable<ChargingPolicy> chargingPolicies = dataManager.GetChargingPolicies();
                return chargingPolicies.ToDictionary(kvp => kvp.ChargingPolicyId, kvp => kvp);
            });
        }

        #endregion

        #region Mappers

        private ChargingPolicyDetail ChargingPolicyDetailMapper(ChargingPolicy chargingPolicy)
        {
            return new ChargingPolicyDetail()
            {
                Entity = chargingPolicy,
                ServiceTypeName = _serviceTypeManager.GetServiceTypeName(chargingPolicy.ServiceTypeId)
            };
        }

        #endregion
    }
}
