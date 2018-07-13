using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPriceListTemplateManager
    {
        #region Public Methods
        public SupplierPriceListTemplate GetSupplierPriceListTemplate(int priceListTemplateId)
        {
            Dictionary<int, SupplierPriceListTemplate> priceListTemplates = GetCachedSupplierPriceListTemplates();
            SupplierPriceListTemplate supplierPriceListTemplate = priceListTemplates.GetRecord(priceListTemplateId);
            return (supplierPriceListTemplate != null) ? supplierPriceListTemplate : null;
        }
        public SupplierPriceListTemplate GetSupplierPriceListTemplateBySupplierId(int supplierId)
        {
            Dictionary<int, SupplierPriceListTemplate> priceListTemplates = GetCachedSupplierPriceListTemplatesBySupplierId();
            SupplierPriceListTemplate supplierPriceListTemplate = priceListTemplates.GetRecord(supplierId);
            return (supplierPriceListTemplate != null) ? supplierPriceListTemplate : null;
        }

        public SupplierPriceListSettings GetSupplierPriceListTemplateSettings(int priceListTemplateId, bool getDraftIfExists)
        {
            Dictionary<int, SupplierPriceListTemplate> priceListTemplates = GetCachedSupplierPriceListTemplates();
            SupplierPriceListTemplate supplierPriceListTemplate = priceListTemplates.GetRecord(priceListTemplateId);

            if (supplierPriceListTemplate == null)
                throw new DataIntegrityValidationException(string.Format("Could not find Supplier Pricelist Template with ID: {0}", priceListTemplateId));

            SupplierPriceListSettings settings = supplierPriceListTemplate.ConfigDetails;
            if (getDraftIfExists && supplierPriceListTemplate.Draft != null)
            {
                settings = supplierPriceListTemplate.Draft;
            }
            return settings;
        }

        public Vanrise.Entities.InsertOperationOutput<SupplierPriceListTemplate> AddSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate)
        {
            InsertOperationOutput<SupplierPriceListTemplate> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SupplierPriceListTemplate>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int supplierPriceListTemplateId = -1;

            ISupplierPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
            bool insertActionSucc = dataManager.InsertSupplierPriceListTemplate(supplierPriceListTemplate, out supplierPriceListTemplateId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                supplierPriceListTemplate.SupplierPriceListTemplateId = supplierPriceListTemplateId;
                insertOperationOutput.InsertedObject = supplierPriceListTemplate;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<SupplierPriceListTemplate> UpdateInputPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate)
        {
            ISupplierPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
            bool updateActionSucc = dataManager.UpdateSupplierPriceListTemplate(supplierPriceListTemplate);
            UpdateOperationOutput<SupplierPriceListTemplate> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SupplierPriceListTemplate>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = supplierPriceListTemplate;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<SupplierPriceListInputConfig> GetSupplierPriceListConfigurationTemplateConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<SupplierPriceListInputConfig>(SupplierPriceListInputConfig.EXTENSION_TYPE);
        }

        public ExcelResult TestConversionForSupplierPriceList(long fileId, DateTime pricelistDate, SupplierPriceListSettings settings)
        {
            SupplierPriceListExecutionContext contextObj = new SupplierPriceListExecutionContext
            {
                InputFileId = fileId,
                PricelistDate = pricelistDate
            };
            ConvertedPriceList convertedPriceList = settings.Execute(contextObj);
            ExcelManager manager = new ExcelManager();

            List<ExportExcelSheet> exportExcelSheets = new List<ExportExcelSheet>();

            if (convertedPriceList.PriceListCodes != null && convertedPriceList.PriceListCodes.Count > 0)
            {
                ExportExcelSheet exportCodeExcelSheet = ConvertPriceListCodesToExcelSheet(convertedPriceList.PriceListCodes);
                if (exportCodeExcelSheet != null)
                    exportExcelSheets.Add(exportCodeExcelSheet);
            }
            if (convertedPriceList.PriceListRates != null && convertedPriceList.PriceListRates.Count > 0)
            {
                ExportExcelSheet exportRateExcelSheet = ConvertPriceListRatesToExcelSheet(convertedPriceList.PriceListRates, "Normal");
                if (exportRateExcelSheet != null)
                    exportExcelSheets.Add(exportRateExcelSheet);
            }
            if (convertedPriceList.PriceListOtherRates != null && convertedPriceList.PriceListOtherRates.Count > 0)
            {
                RateTypeManager rateTypeManager = new RateTypeManager();

                foreach (var otherRates in convertedPriceList.PriceListOtherRates)
                {
                    var rateType = rateTypeManager.GetRateType(otherRates.Key);
                    ExportExcelSheet exportExcelSheet = ConvertPriceListRatesToExcelSheet(otherRates.Value, rateType.Name);
                    exportExcelSheets.Add(exportExcelSheet);
                }
            }
            if (convertedPriceList.PriceListServices != null && convertedPriceList.PriceListServices.Count > 0)
            {
                ExportExcelSheet exportExcelSheet = ConvertPriceListServicesToExcelSheet(convertedPriceList.PriceListServices);
                exportExcelSheets.Add(exportExcelSheet);
            }
            return manager.ExportExcel(exportExcelSheets);
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierPriceListTemplateDataManager _dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSupplierPriceListTemplatesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, SupplierPriceListTemplate> GetCachedSupplierPriceListTemplates()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierPriceListTemplates", () =>
            {
                ISupplierPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
                IEnumerable<SupplierPriceListTemplate> priceListTemplates = dataManager.GetSupplierPriceListTemplates();
                return priceListTemplates.ToDictionary(kvp => kvp.SupplierPriceListTemplateId, kvp => kvp);
            });
        }
        private Dictionary<int, SupplierPriceListTemplate> GetCachedSupplierPriceListTemplatesBySupplierId()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSupplierPriceListTemplatesBySupplierId", () =>
            {
                ISupplierPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
                IEnumerable<SupplierPriceListTemplate> priceListTemplates = dataManager.GetSupplierPriceListTemplates();
                return priceListTemplates.ToDictionary(kvp => kvp.SupplierId, kvp => kvp);
            });
        }


        private ExportExcelSheet ConvertPriceListCodesToExcelSheet(List<PriceListCode> priceListCodes)
        {
            ExportExcelSheet exportExcelSheet = null;
            if (priceListCodes.Count > 0)
            {
                List<ExportExcelHeaderCell> exportExcelHeaderCell = new List<ExportExcelHeaderCell>(){
                    new ExportExcelHeaderCell{ Title = "Zone" },
                    new ExportExcelHeaderCell{ Title = "Code" },
                    new ExportExcelHeaderCell{ Title = "Effective Date" ,CellType = ExcelCellType.DateTime ,DateTimeType = DateTimeType.DateTime}
                };
                exportExcelSheet = CreateExcelSheet("Codes", exportExcelHeaderCell);
                foreach (var priceListCode in priceListCodes)
                {
                    exportExcelSheet.Rows.Add(new ExportExcelRow
                    {
                        Cells = new List<ExportExcelCell>(){
                            new ExportExcelCell{ Value = priceListCode.ZoneName},
                            new ExportExcelCell{ Value = priceListCode.Code },
                            new ExportExcelCell{ Value = priceListCode.EffectiveDate}
                        }
                    });
                }
            }

            return exportExcelSheet;
        }
        private ExportExcelSheet ConvertPriceListRatesToExcelSheet(List<PriceListRate> priceListRates, string rateTypeName)
        {
            ExportExcelSheet exportExcelSheet = null;
            if (priceListRates.Count > 0)
            {
                List<ExportExcelHeaderCell> exportExcelHeaderCell = new List<ExportExcelHeaderCell>(){
                        new ExportExcelHeaderCell{ Title = "Zone" },
                        new ExportExcelHeaderCell{ Title = string.Format("{0} Rate",rateTypeName) },
                        new ExportExcelHeaderCell{ Title = "Effective Date" ,CellType = ExcelCellType.DateTime ,DateTimeType = DateTimeType.DateTime}
                    };
                exportExcelSheet = CreateExcelSheet(string.Format("{0} Rates", rateTypeName), exportExcelHeaderCell);
                foreach (var priceListRate in priceListRates)
                {
                    exportExcelSheet.Rows.Add(new ExportExcelRow
                    {
                        Cells = new List<ExportExcelCell>(){
                            new ExportExcelCell{ Value = priceListRate.ZoneName},
                            new ExportExcelCell{ Value = priceListRate.Rate },
                            new ExportExcelCell{ Value = priceListRate.EffectiveDate}
                        }
                    });
                }

            }

            return exportExcelSheet;
        }

        private ExportExcelSheet ConvertPriceListServicesToExcelSheet(List<PriceListZoneService> priceListZoneServices)
        {
            ExportExcelSheet exportExcelSheet = null;
            if (priceListZoneServices.Count > 0)
            {
                List<ExportExcelHeaderCell> exportExcelHeaderCell = new List<ExportExcelHeaderCell>(){
                        new ExportExcelHeaderCell{ Title = "Zone" },
                        new ExportExcelHeaderCell{ Title = string.Format("Services") },
                        new ExportExcelHeaderCell{ Title = "Effective Date" ,CellType = ExcelCellType.DateTime ,DateTimeType = DateTimeType.DateTime}
                    };
                exportExcelSheet = CreateExcelSheet(string.Format("Services"), exportExcelHeaderCell);
                foreach (var priceListZoneService in priceListZoneServices)
                {
                    exportExcelSheet.Rows.Add(new ExportExcelRow
                    {
                        Cells = new List<ExportExcelCell>(){
                            new ExportExcelCell{ Value = priceListZoneService.ZoneName},
                            new ExportExcelCell{ Value = priceListZoneService.ZoneServiceConfigId },
                            new ExportExcelCell{ Value = priceListZoneService.EffectiveDate}
                        }
                    });
                }

            }
            return exportExcelSheet;
        }

        private ExportExcelSheet CreateExcelSheet(string sheetName, List<ExportExcelHeaderCell> exportExcelHeaderCell)
        {
            ExportExcelHeader exportExcelHeader = new ExportExcelHeader();
            exportExcelHeader.Cells = exportExcelHeaderCell;
            return new ExportExcelSheet
            {
                SheetName = sheetName,
                Header = exportExcelHeader,
                Rows = new List<ExportExcelRow>(),
            };
        }
        #endregion
    }
}
