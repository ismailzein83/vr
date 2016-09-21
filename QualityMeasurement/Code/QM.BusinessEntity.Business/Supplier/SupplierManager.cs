using Aspose.Cells;
using QM.BusinessEntity.Data;
using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            
            SupplierExcelExportHandler supplierExcel = new SupplierExcelExportHandler(input.Query);

            ResultProcessingHandler<SupplierDetail> handler = new ResultProcessingHandler<SupplierDetail>()
            {
                ExportExcelHandler = supplierExcel
            };
            
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input,
                allSuppliers.ToBigResult(input, filterExpression, SupplierDetailMapper), handler);
        }
        public Supplier GetSupplier(int supplierId)
        {
            var suppliers = GetCachedSuppliers();
            return suppliers.GetRecord(supplierId);
        }
        public Dictionary<string, long> GetExistingItemIds()
        {
            Dictionary<string, long> existingItemIds = new Dictionary<string, long>();
            foreach (var item in GetCachedSuppliers())
            {
                if (item.Value.SourceId != null)
                {
                    //if (sourceItemIds.Contains(item.Value.SourceId))
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

            AssignSupplierId(supplier);

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            UpdateSettings(supplier, false);

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
            UpdateSettings(supplier, true);

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
        
        public void AddSupplierFromSource(Supplier supplier)
        {
            AssignSupplierId(supplier);
            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            UpdateSettings(supplier, false);

            dataManager.InsertSupplierFromeSource(supplier);
        }
        public void UpdateSupplierFromSource(Supplier supplier)
        {
            ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            UpdateSettings(supplier, true);

            dataManager.UpdateSupplierFromeSource(supplier);
        }
        public IEnumerable<SourceSupplierReaderConfig> GetSupplierSourceTemplates()
        {

            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SourceSupplierReaderConfig>(SourceSupplierReaderConfig.EXTENSION_TYPE);
        }

        public Supplier GetSupplierbySourceId(string sourceSupplierId)
        {
            var allSuppliers = GetCachedSuppliers();            
            Func<Supplier, bool> filterExpression = (sup) => (sup.SourceId ==  sourceSupplierId);
            return allSuppliers.FindRecord(filterExpression);
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
           
            for (int i = 1; i < supplierDataTable.Rows.Count; i++)
            {
                string supplierName = supplierDataTable.Rows[i][0].ToString().ToLower();
                string prefix = supplierDataTable.Rows[i][1].ToString();

                Supplier supplier = GetSupplierByName(supplierName);
                if (supplier == null)
                {
                    supplier = new Supplier();
                    AssignSupplierId(supplier);
                    supplier.Name = supplierName;

                    if (supplier.Settings == null)
                        supplier.Settings = new SupplierSettings();
                    supplier.Settings.Prefix = prefix;

                    UpdateSettings(supplier, false);
                    
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
                        if (supplier.Settings == null)
                            supplier.Settings = new SupplierSettings();
                        supplier.Settings.Prefix = prefix;

                        UpdateSettings(supplier, true);
                        
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


        public Supplier GetSupplierByName(string supplierName)
        {
            return GetCachedSuppliers().FindRecord(it => it.Name.ToLower().Equals(supplierName));
        }


        #region Private Members

        private class SupplierRequestHandler : BigDataRequestHandler<SupplierQuery, Supplier, SupplierDetail>
        {
            public override IEnumerable<Supplier> RetrieveAllData(DataRetrievalInput<SupplierQuery> input)
            {
                throw new ArgumentException("query.ReportType is invalid");
            }
            protected override ResultProcessingHandler<SupplierDetail> GetResultProcessingHandler(DataRetrievalInput<SupplierQuery> input, BigResult<SupplierDetail> bigResult)
            {
                return new ResultProcessingHandler<SupplierDetail>
                {
                    ExportExcelHandler = new SupplierExcelExportHandler(input.Query)
                };
            }

            public override SupplierDetail EntityDetailMapper(Supplier entity)
            {
                return new SupplierDetail()
                {
                    Entity = entity
                };
            }
        }

        private class SupplierExcelExportHandler : ExcelExportHandler<SupplierDetail>
        {
            private SupplierQuery _query;
            public SupplierExcelExportHandler(SupplierQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierDetail> context)
            {
                if (context.BigResult == null)
                    throw new ArgumentNullException("context.BigResult");
                if (context.BigResult.Data == null)
                    throw new ArgumentNullException("context.BigResult.Data");
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Prefix" });

                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.Rows.Add(row);
                    if (record.Entity.Settings != null)
                    {
                        row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                        row.Cells.Add(new ExportExcelCell { Value = record.Entity.Settings.Prefix });
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private void UpdateSettings(Supplier supplier, bool isUpdate)
        {
            if (supplier.Settings == null)
                supplier.Settings = new SupplierSettings();


            if (isUpdate)
            {
                var existingSupplier = GetSupplier(supplier.SupplierId);
                if (existingSupplier == null)
                    throw new NullReferenceException(String.Format("existingSupplier {0}", supplier.SupplierId));
                if (existingSupplier.Settings == null)
                    throw new NullReferenceException(String.Format("existingSupplier.Settings {0}", supplier.SupplierId));
                supplier.Settings.ExtendedSettings = existingSupplier.Settings.ExtendedSettings;
            }
            else
            {
                if (supplier.Settings.ExtendedSettings == null)
                    supplier.Settings.ExtendedSettings = new Dictionary<string, object>();
            }

            IEnumerable<Type> extendedSettingsBehaviorsImplementations =
                Utilities.GetAllImplementations<ExtendedSupplierSettingBehavior>();
            if (extendedSettingsBehaviorsImplementations != null)
            {
                foreach (var extendedSettingsBehaviorType in extendedSettingsBehaviorsImplementations)
                {
                    var extendedSettingsBehavior =
                        Activator.CreateInstance(extendedSettingsBehaviorType) as ExtendedSupplierSettingBehavior;
                    extendedSettingsBehavior.ApplyExtendedSettings(new ApplyExtendedSupplierSettingsContext
                    {
                        Supplier = supplier
                    });
                }
            }
        }

        private static void AssignSupplierId(Supplier supplier)
        {
            long startingId;
            ReserveIDRange(1, out startingId);
            supplier.SupplierId = (int)startingId;
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
