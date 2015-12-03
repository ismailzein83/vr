using QM.BusinessEntity.Data;
using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public class SupplierManager
    {
        public Vanrise.Entities.InsertOperationOutput<SupplierDetail> AddSupplier(Supplier supplier)
        {
            Vanrise.Entities.InsertOperationOutput<SupplierDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SupplierDetail>();
            long startingId;
            ReserveIDRange(1, out startingId);
            supplier.SupplierId = (int)startingId;

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            if(supplier.Settings != null &&supplier.Settings.ExtendedSettings != null)
            {
                foreach(var extendedSetting in supplier.Settings.ExtendedSettings.Values)
                {
                    extendedSetting.Apply(supplier);
                }
            }

            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            bool insertActionSucc = dataManager.Insert(supplier);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                SupplierDetail supplierDetail = SupplierDetailMapper(supplier);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = supplierDetail;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SupplierDetail> UpdateCarrierAccount(Supplier supplier)
        {
            if (supplier.Settings != null && supplier.Settings.ExtendedSettings != null)
            {
                foreach (var extendedSetting in supplier.Settings.ExtendedSettings.Values)
                {
                    extendedSetting.Apply(supplier);
                }
            }

            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();

            bool updateActionSucc = dataManager.Update(supplier);
            Vanrise.Entities.UpdateOperationOutput<SupplierDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SupplierDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                SupplierDetail supplierDetail = SupplierDetailMapper(supplier);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = supplierDetail;
            }

            return updateOperationOutput;
        }

        public void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(this.GetType(), nbOfIds, out startingId);
        }

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSuppliersUpdated(ref _updateHandle);
            }
        }

        private SupplierDetail SupplierDetailMapper(Supplier supplier)
        {
            var supplierDetail = new SupplierDetail
            {
                Entity = supplier
            };
            return supplierDetail;
        }

        #endregion
    }
}
