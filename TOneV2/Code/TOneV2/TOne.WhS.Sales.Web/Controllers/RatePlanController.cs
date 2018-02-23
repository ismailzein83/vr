using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RatePlan")]
    public class RatePlanController : BaseAPIController
    {
        [HttpGet]
        [Route("ValidateCustomer")]
        public bool ValidateCustomer(int customerId, DateTime effectiveOn)
        {
            var manager = new RatePlanManager();
            return manager.ValidateCustomer(customerId, effectiveOn);
        }

        [HttpPost]
        [Route("GetZoneLetters")]
        public IEnumerable<char> GetZoneLetters(ZoneLettersInput input)
        {
            var manager = new RatePlanManager();
            return manager.GetZoneLetters(input);
        }

        [HttpPost]
        [Route("GetZoneItems")]
        public IEnumerable<ZoneItem> GetZoneItems(GetZoneItemsInput input)
        {
            var manager = new RatePlanManager();
            return manager.GetZoneItems(input);
        }

        [HttpPost]
        [Route("GetZoneItem")]
        public ZoneItem GetZoneItem(ZoneItemInput input)
        {
            var manager = new RatePlanManager();
            return manager.GetZoneItem(input);
        }

        [HttpGet]
        [Route("GetDefaultItem")]
        public DefaultItem GetDefaultItem(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            var manager = new DefaultItemManager();
            return manager.GetDefaultItem(ownerType, ownerId, effectiveOn);
        }

        [HttpGet]
        [Route("GetCountryChanges")]
        public CountryChanges GetCountryChanges(int customerId)
        {
            var manager = new RatePlanDraftManager();
            return manager.GetCountryChanges(customerId);
        }

        [HttpGet]
        [Route("GetTQIMethods")]
        public IEnumerable<TQIMethodConfig> GetTQIMethods()
        {
            var manager = new RatePlanExtensionConfigManager();
            return manager.GetTQIMethods();
        }

        [HttpPost]
        [Route("GetTQIEvaluatedRate")]
        public TQIEvaluatedRate GetTQIEvaluatedRate(TQIEvaluatedRateInput evaluatedRateInput)
        {
            TQIManager tqiManager = new TQIManager();
            return tqiManager.Evaluate(evaluatedRateInput.TQIMethod, evaluatedRateInput.RPRouteDetail);
        }

        [HttpPost]
        [Route("GetTQISuppliersInfo")]
        public TQISupplierInfoWithSummary GetTQISuppliersInfo(TQISupplierInfoQuery input)
        {
            TQIManager tqiManager = new TQIManager();
            return tqiManager.GetTQISuppliersInfo(input);
        }

        [HttpGet]
        [Route("GetCostCalculationMethodTemplates")]
        public IEnumerable<CostCalculationMethodSetting> GetCostCalculationMethodTemplates()
        {
            var manager = new RatePlanExtensionConfigManager();
            return manager.GetCostCalculationMethodTemplates();
        }

        [HttpGet]
        [Route("GetRateCalculationMethodTemplates")]
        public IEnumerable<RateCalculationMethodSetting> GetRateCalculationMethodTemplates()
        {
            var manager = new RatePlanExtensionConfigManager();
            return manager.GetRateCalculationMethodTemplates();
        }

        [HttpGet]
        [Route("GetBulkActionTypeExtensionConfigs")]
        public IEnumerable<BulkActionTypeSettings> GetBulkActionTypeExtensionConfigs(SalePriceListOwnerType ownerType)
        {
            var manager = new RatePlanExtensionConfigManager();
            return manager.GetBulkActionTypeExtensionConfigs(ownerType);
        }

        [HttpGet]
        [Route("GetBulkActionZoneFilterTypeExtensionConfigs")]
        public IEnumerable<BulkActionZoneFilterTypeSettings> GetBulkActionZoneFilterTypeExtensionConfigs()
        {
            var manager = new RatePlanExtensionConfigManager();
            return manager.GetBulkActionZoneFilterTypeExtensionConfigs();
        }

        [HttpPost]
        [Route("SaveChanges")]
        public void SaveChanges(SaveChangesInput input)
        {
            var manager = new RatePlanDraftManager();
            manager.SaveDraft(input.OwnerType, input.OwnerId, input.NewChanges);
        }

        [HttpPost]
        [Route("TryApplyCalculatedRates")]
        public CalculatedRates TryApplyCalculatedRates(TryApplyCalculatedRatesInput input)
        {
            var manager = new RatePlanPricingManager();
            return manager.TryApplyCalculatedRates(input);
        }

        [HttpPost]
        [Route("ApplyCalculatedRates")]
        public void ApplyCalculatedRates(ApplyCalculatedRatesInput input)
        {
            var manager = new RatePlanPricingManager();
            manager.ApplyCalculatedRates(input);
        }

        [HttpPost]
        [Route("ApplyBulkActionToDraft")]
        public void ApplyBulkActionToDraft(ApplyActionToDraftInput input)
        {
            var manager = new RatePlanManager();
            manager.ApplyBulkActionToDraft(input);
        }

        [HttpGet]
        [Route("CheckIfDraftExists")]
        public bool CheckIfDraftExists(SalePriceListOwnerType ownerType, int ownerId)
        {
            var manager = new RatePlanDraftManager();
            return manager.DoesDraftExist(ownerType, ownerId);
        }

        [HttpGet]
        [Route("DeleteDraft")]
        public bool DeleteDraft(SalePriceListOwnerType ownerType, int ownerId)
        {
            var manager = new RatePlanDraftManager();
            return manager.DeleteDraft(ownerType, ownerId);
        }

        [HttpGet]
        [Route("GetRatePlanSettingsData")]
        public RatePlanSettingsData GetRatePlanSettingsData()
        {
            var manager = new RatePlanManager();
            return manager.GetRatePlanSettingsData();
        }

        [HttpGet]
        [Route("GetFollowPublisherRatesBED")]
        public bool GetFollowPublisherRatesBED()
        {
            var manager = new RatePlanManager();
            return manager.GetFollowPublisherRatesBED();
        }
        [HttpGet]
        [Route("GetFollowPublisherRoutingProduct")]
        public bool GetFollowPublisherRoutingProduct()
        {
            var manager = new RatePlanManager();
            return manager.GetFollowPublisherRoutingProduct();
        }

        [HttpGet]
        [Route("GetSaleAreaSettingsData")]
        public SaleAreaSettingsData GetSaleAreaSettingsData()
        {
            var manager = new RatePlanManager();
            return manager.GetSaleAreaSettingsData();
        }

        [HttpGet]
        [Route("GetPricingSettings")]
        public PricingSettings GetPricingSettings(SalePriceListOwnerType ownerType, int ownerId)
        {
            return TOne.WhS.Sales.Business.UtilitiesManager.GetPricingSettings(ownerType, ownerId);
        }

        [HttpGet]
        [Route("GetDraftCurrencyId")]
        public int? GetDraftCurrencyId(SalePriceListOwnerType ownerType, int ownerId)
        {
            var manager = new RatePlanDraftManager();
            return manager.GetDraftCurrencyId(ownerType, ownerId);
        }

        [HttpGet]
        [Route("GetDraftSubscriberOwnerEntities")]
        public List<SubscriberOwnerEntity> GetDraftSubscriberOwnerEntities(SalePriceListOwnerType ownerType, int ownerId)
        {
            var manager = new RatePlanDraftManager();
            return manager.GetDraftSubscriberOwnerEntities(ownerType, ownerId);
        }

        [HttpPost]
        [Route("DefineNewRatesConvertedToCurrency")]
        public void DefineNewRatesConvertedToCurrency(DefineNewRatesConvertedToCurrencyInput input)
        {
            new RatePlanDraftManager().DefineNewRatesConvertedToCurrency(input);
        }

        [HttpPost]
        [Route("GetCustomerDefaultInheritedService")]
        public SaleEntityService GetCustomerDefaultInheritedService(GetCustomerDefaultInheritedServiceInput input)
        {
            var manager = new DefaultItemManager();
            return manager.GetCustomerDefaultInheritedService(input);
        }

        [HttpPost]
        [Route("GetZoneInheritedService")]
        public SaleEntityService GetZoneInheritedService(GetZoneInheritedServiceInput input)
        {
            var manager = new RatePlanZoneManager();
            return manager.GetZoneInheritedService(input);
        }

        [HttpPost]
        [Route("GetFilteredSoldCountries")]
        public object GetFilteredSoldCountries(Vanrise.Entities.DataRetrievalInput<SoldCountryQuery> input)
        {
            var manager = new SoldCountryManager();
            return base.GetWebResponse(input, manager.GetFilteredSoldCountries(input));
        }

        [HttpPost]
        [Route("ImportRatePlan")]
        public ImportRatePlanResult ImportRatePlan(ImportRatePlanInput input)
        {
            return new RatePlanManager().ImportRatePlan(input);
        }

        [HttpGet]
        [Route("DownloadImportRatePlanResult")]
        public object DownloadImportRatePlanResult(long fileId)
        {
            byte[] fileContent = new RatePlanManager().DownloadImportRatePlanResult(fileId);
            return GetExcelResponse(fileContent, "Imported Rate Plan Result.xls");
        }

        [HttpGet]
        [Route("DownloadImportRatePlanTemplate")]
        public object DownloadImportRatePlanTemplate()
        {
            string templateRelativePath = "~/Client/Modules/WhS_Sales/Templates/Import Rate Plan Template.xlsx";
            string templateAbsolutePath = HttpContext.Current.Server.MapPath(templateRelativePath);
            byte[] templateBytes = File.ReadAllBytes(templateAbsolutePath);
            byte[] templateWithDateFormatBytes = new RatePlanManager().GetImportTemplateFileWithSystemDateFormat(templateBytes);
            MemoryStream memoryStream = new System.IO.MemoryStream();
            memoryStream.Write(templateWithDateFormatBytes, 0, templateWithDateFormatBytes.Length);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memoryStream, "Import Rate Plan Template.xlsx");
        }

        [HttpPost]
        [Route("ValidateBulkActionZones")]
        public object ValidateBulkActionZones(BulkActionZoneValidationInput input)
        {
            return new RatePlanManager().ValidateBulkActionZones(input);
        }

        [HttpPost]
        [Route("ValidateImportedData")]
        public ImportedDataValidationResult ValidateImportedData(ImportedDataValidationInput input)
        {
            return new RatePlanManager().ValidateImportedData(input);
        }

        [HttpGet]
        [Route("GetOwnerInfo")]
        public OwnerInfo GetOwnerInfo(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            return new RatePlanManager().GetOwnerInfo(ownerType, ownerId, effectiveOn);
        }

        [HttpGet]
        [Route("GetSystemDateFormat")]
        public string GetSystemDateFormat()
        {
            return new RatePlanManager().GetSystemDateTimeFormat(Vanrise.Entities.DateTimeType.Date);
        }

        [HttpPost]
        [Route("GetSubscriberOwners")]
        public IEnumerable<CarrierAccountInfo> GetSubscriberOwners(GetSubscriberOwnersInput getSubscriberOwnersInput)
        {
            return new RatePlanManager().GetSubscriberOwners(getSubscriberOwnersInput);
        }
        
    }
}