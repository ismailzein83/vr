using QM.BusinessEntity.Data;
using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace QM.BusinessEntity.Business
{
    public class SupplierManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierDetail> GetFilteredSuppliers(Vanrise.Entities.DataRetrievalInput<SupplierQuery> input)
        {
            var allSuppliers = GetCachedSuppliers();

            Func<Supplier, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSuppliers.ToBigResult(input, filterExpression, SupplierDetailMapper));
        }

        public Supplier GetSupplier(int supplierId)
        {
            var suppliers = GetCachedSuppliers();
            return suppliers.GetRecord(supplierId);
        }

        public IEnumerable<SupplierInfo> GetSuppliersInfo()
        {
            var suppliers = GetCachedSuppliers();
            return suppliers.MapRecords(SupplierInfoMapper);
        }

        public Vanrise.Entities.InsertOperationOutput<SupplierDetail> AddSupplier(Supplier supplier)
        {
            Vanrise.Entities.InsertOperationOutput<SupplierDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SupplierDetail>();

            long startingId;
            ReserveIDRange(1, out startingId);
            supplier.SupplierId = (int)startingId;

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int supplierId = -1;
            if (supplier.Settings != null && supplier.Settings.ExtendedSettings != null)
            {
                foreach (var extendedSetting in supplier.Settings.ExtendedSettings)
                {
                    extendedSetting.Apply(supplier);
                }
            }

            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            bool insertActionSucc = dataManager.Insert(supplier, out supplierId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                supplier.SupplierId = supplierId;
                insertOperationOutput.InsertedObject = SupplierDetailMapper(supplier);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SupplierDetail> UpdateSupplier(Supplier supplier)
        {
            if (supplier.Settings != null && supplier.Settings.ExtendedSettings != null)
            {
                foreach (var extendedSetting in supplier.Settings.ExtendedSettings)
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
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }


        #region Private Members

        public Dictionary<int, Supplier> GetCachedSuppliers()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSuppliers",
               () =>
               {
                   ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
                   IEnumerable<Supplier> suppliers = dataManager.GetSuppliers();
                   return suppliers.ToDictionary(cn => cn.SupplierId, cn => cn);
               });
        }

        private SupplierInfo SupplierInfoMapper(Supplier supplier)
        {
            return new SupplierInfo()
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
            };
        }

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

        private void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(this.GetType(), nbOfIds, out startingId);
        }

        #endregion

    }
}
