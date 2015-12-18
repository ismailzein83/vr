using Aspose.Cells;
using QM.BusinessEntity.Data;
using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using System.Drawing;

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

        public string AddSuppliers(int fileId)
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

            if (wbk.Worksheets[0].Cells.MaxDataRow > -1 && wbk.Worksheets[0].Cells.MaxDataColumn > -1)
                supplierDataTable = wbk.Worksheets[0].Cells.ExportDataTableAsString(0, 0, wbk.Worksheets[0].Cells.MaxDataRow + 1, wbk.Worksheets[0].Cells.MaxDataColumn + 1);

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

                    ISupplierDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
                    bool insertActionSucc = dataManager.Insert(supplier);
                    if (insertActionSucc)
                        insertedCount++;
                    else
                        notInsertedCount++;
                }
                else
                {
                    notInsertedCount++;
                }
            }

            message = String.Format("{0} suppliers added and {1} already exists", insertedCount, notInsertedCount);

            return message;
        }


        public IDataRetrievalResult<T> ExportTemplate<T>()
        {
            //default Export is Excel

            ExcelResult<T> excelResult = new ExcelResult<T>();


            Workbook wbk = new Workbook();
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            wbk.Worksheets.Clear();
            Worksheet workSheet = wbk.Worksheets.Add("Result");
            int rowIndex = 0;
            int colIndex = 0;

            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo entityProperty = null;
            PropertyInfo[] entityProperties = null;

            //filling header
            foreach (var prop in properties)
            {
                if (prop.Name == "Entity")
                {
                    entityProperty = prop;
                    entityProperties = prop.PropertyType.GetProperties();
                    continue;
                }
                workSheet.Cells.SetColumnWidth(colIndex, 20);
                workSheet.Cells[rowIndex, colIndex].PutValue(prop.Name);
                Cell cell = workSheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }
            if (entityProperties != null)
            {
                foreach (var prop in entityProperties)
                {
                    workSheet.Cells.SetColumnWidth(colIndex, 20);
                    workSheet.Cells[rowIndex, colIndex].PutValue(prop.Name);
                    Cell cell = workSheet.Cells.GetCell(rowIndex, colIndex);
                    Style style = cell.GetStyle();
                    style.Font.Name = "Times New Roman";
                    style.Font.Color = Color.FromArgb(255, 0, 0); ;
                    style.Font.Size = 14;
                    style.Font.IsBold = true;
                    cell.SetStyle(style);
                    colIndex++;
                }
            }
            rowIndex++;
            colIndex = 0;

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = wbk.SaveToStream();

            excelResult.ExcelFileStream = memoryStream;
            wbk.Save("D:\\book1.xlsx", SaveFormat.Xlsx);
            return excelResult;
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
