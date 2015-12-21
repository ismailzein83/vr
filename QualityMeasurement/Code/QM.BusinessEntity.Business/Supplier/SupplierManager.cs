using Aspose.Cells;
using QM.BusinessEntity.Data;
using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;

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
        public Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            Dictionary<string, long> existingItemIds = new Dictionary<string, long>();
            foreach (var item in GetCachedSuppliers())
            {
                if (item.Value.SourceId != null)
                {
                    if (sourceItemIds.Contains(item.Value.SourceId))
                        existingItemIds.Add(item.Value.SourceId, (long)item.Value.SupplierId);
                }
            }
            return existingItemIds;
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

            if (supplier.Settings != null && supplier.Settings.ExtendedSettings != null)
            {
                foreach (var extendedSetting in supplier.Settings.ExtendedSettings)
                {
                    extendedSetting.Apply(supplier);
                }
            }

            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            bool insertActionSucc = dataManager.Insert(supplier);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
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

        public void AddSupplierFromeSource(Supplier supplier)
        {
            long startingId;
            ReserveIDRange(1, out startingId);
            supplier.SupplierId = (int)startingId;
            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            dataManager.InsertSupplierFromeSource(supplier);
        }
        public void UpdateSupplierFromeSource(Supplier supplier)
        {

            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            dataManager.UpdateSupplierFromeSource(supplier);
        }
        public List<Vanrise.Entities.TemplateConfig> GetSupplierSourceTemplates()
        {

            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SourceSupplierReaderConfigType);
        }

        
        public string AddSuppliers(int fileId, bool AllowUpdateIfExisting)
        {
            DataTable supplierDataTable = new DataTable();
            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFile(fileId).Content;
            var fileStream = new System.IO.MemoryStream(bytes);
            ExportTableOptions options = new ExportTableOptions();
            options.CheckMixedValueType = true;
            Workbook wbk = new Workbook(fileStream);
            wbk.CalculateFormula();
            string message = "";
            int insertedCount = 0;
            int notInsertedCount = 0;
            int updatedCount = 0;

            if (wbk.Worksheets[0].Cells.MaxDataRow > -1 && wbk.Worksheets[0].Cells.MaxDataColumn > -1)
                supplierDataTable = wbk.Worksheets[0].Cells.ExportDataTableAsString(0, 0, wbk.Worksheets[0].Cells.MaxDataRow + 1, wbk.Worksheets[0].Cells.MaxDataColumn + 1);


            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            IEnumerable<Type> implementations = Utilities.GetAllImplementations<ExtendedSupplierSetting>();

            List<string> extendedFields = new List<string>();
            for (int i = 1; i < supplierDataTable.Columns.Count; i++)
                extendedFields.Add(supplierDataTable.Rows[0][i].ToString());

            for (int i = 1; i < supplierDataTable.Rows.Count; i++)
            {
                Supplier supplier = GetCachedSuppliers().FindRecord(it => it.Name.ToLower().Equals(supplierDataTable.Rows[i][0].ToString().ToLower()));
                if (supplier == null)
                {
                    supplier = new Supplier();
                    long startingId;
                    ReserveIDRange(1, out startingId);
                    supplier.SupplierId = (int)startingId;
                    supplier.Name = supplierDataTable.Rows[i][0].ToString();
                    ApplySettings(supplierDataTable, ref insertedCount, ref notInsertedCount, ref updatedCount, dataManager, implementations, extendedFields, i, ref supplier);

                    bool insertActionSucc = dataManager.Insert(supplier);
                    if (insertActionSucc)
                        insertedCount++;
                    else
                        notInsertedCount++;


                }
                else
                {
                    if (AllowUpdateIfExisting)
                    {
                        ApplySettings(supplierDataTable, ref insertedCount, ref notInsertedCount, ref updatedCount, dataManager, implementations, extendedFields, i, ref supplier);

                        bool updateActionSucc = dataManager.Update(supplier);
                        if (updateActionSucc)
                            updatedCount++;
                        else
                            notInsertedCount++;
                    }
                    else
                        notInsertedCount++;
                }
            }

            message = String.Format("{0} suppliers added, {1} already exists, and {2} updated.", insertedCount, notInsertedCount, updatedCount);

            return message;
        }

        private static void ApplySettings(DataTable supplierDataTable, ref int insertedCount, ref int notInsertedCount, ref int updatedCount, ISupplierDataManager dataManager, IEnumerable<Type> implementations, List<string> extendedFields, int i, ref Supplier supplier)
        {
            if (supplier.Settings == null)
            {
                supplier.Settings = new SupplierSettings();
                supplier.Settings.ExtendedSettings = new List<ExtendedSupplierSetting>();

                foreach (Type type in implementations)
                {
                    ExtendedSupplierSetting baseClass = Activator.CreateInstance(type) as ExtendedSupplierSetting;
                    if (baseClass == null)
                        throw new Exception(String.Format("invalid logger type {0}", baseClass.GetType()));

                    Dictionary<string, object> excelFields = new Dictionary<string, object>();

                    for (int j = 1; j < supplierDataTable.Columns.Count; j++)
                        excelFields.Add(extendedFields[j - 1], supplierDataTable.Rows[i][j].ToString());

                    baseClass.ApplyExcelFields(supplier, excelFields);

                    supplier.Settings.ExtendedSettings.Add(baseClass);
                }
            }
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

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(SupplierManager), nbOfIds, out startingId);
        }

        #endregion

    }
}
