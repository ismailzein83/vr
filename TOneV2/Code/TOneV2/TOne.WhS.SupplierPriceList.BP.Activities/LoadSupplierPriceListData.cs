using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
	public class LoadSupplierPriceListData : CodeActivity
	{
		#region Input Argument

		[RequiredArgument]
		public InArgument<long> FileId { get; set; }

		[RequiredArgument]
		public InArgument<int> SupplierPriceListTemplateId { get; set; }

		[RequiredArgument]
		public InArgument<int> CurrencyId { get; set; }

		[RequiredArgument]
		public InArgument<DateTime> PriceListDate { get; set; }

		[RequiredArgument]
		public InArgument<int?> ReceivedPricelistRecordId { get; set; }

		[RequiredArgument]
		public InArgument<bool> IsAutoImport { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

		[RequiredArgument]
		public OutArgument<AllImportedCodes> AllImportedCodes { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<PriceListCode>> FilteredImportedCodes { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<ImportedZoneService>> ImportedZonesServices { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<int>> ImportedRateTypeIds { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<int>> ImportedServiceTypeIds { get; set; }

		[RequiredArgument]
		public OutArgument<bool> IncludeServices { get; set; }

		[RequiredArgument]
		public OutArgument<DateTime> MinimumDate { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			int currencyId = this.CurrencyId.Get(context);
			int supplierPriceListTemplateId = SupplierPriceListTemplateId.Get(context);
			DateTime pricelistDate = PriceListDate.Get(context);
			int? receivedPricelistRecordId = this.ReceivedPricelistRecordId.Get(context);
			bool isAutoImport = this.IsAutoImport.Get(context);

			DateTime startReading = DateTime.Now;

			ConvertedPriceList convertedPriceList = null;
			SupplierPriceListTemplateManager supplierPriceListTemplateManager = new Business.SupplierPriceListTemplateManager();

			try
			{
				SupplierPriceListSettings settings = supplierPriceListTemplateManager.GetSupplierPriceListTemplateSettings(supplierPriceListTemplateId, true);
				SupplierPriceListExecutionContext contextObj = new SupplierPriceListExecutionContext
				{
					InputFileId = FileId.Get(context),
					PricelistDate = pricelistDate
				};
				convertedPriceList = settings.Execute(contextObj);
			}
			catch (Exception ex)
			{
				if (isAutoImport)
				{
					IReceivedPricelistManager manager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
					manager.UpdateReceivedPricelistStatus(receivedPricelistRecordId.Value, ReceivedPricelistStatus.FailedDueToConfigurationError,new List<SPLImportErrorDetail>() { new SPLImportErrorDetail() { Message = ex.Message} });
					var receivedSupplierPricelistManager = new ReceivedSupplierPricelistManager();
					receivedSupplierPricelistManager.SendMailToSupplier(receivedPricelistRecordId.Value, AutoImportEmailTypeEnum.Failed);
				}
				throw;
			}

			DateTime minimumDate = DateTime.MinValue;

			List<ImportedCode> importedCodesList = new List<ImportedCode>();
			List<ImportedRate> importedRatesList = new List<ImportedRate>();
			List<ImportedZoneService> importedZonesServicesList = new List<ImportedZoneService>();

			BusinessEntity.Business.CodeGroupManager codeGroupManager = new BusinessEntity.Business.CodeGroupManager();

			foreach (var priceListCode in convertedPriceList.PriceListCodes)
			{


				if (IsEffectiveDateLessThanMinDate(minimumDate, priceListCode.EffectiveDate))
					minimumDate = priceListCode.EffectiveDate.Value;

				string codeValue = priceListCode.Code != null ? priceListCode.Code.Trim() : null;
				string zoneNameValue = priceListCode.ZoneName != null ? priceListCode.ZoneName.Trim() : null;

				TOne.WhS.BusinessEntity.Entities.CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(codeValue);
				importedCodesList.Add(new ImportedCode
				{
					Code = codeValue,
					CodeGroup = codeGroup,
					ZoneName = zoneNameValue,
					BED = (priceListCode.EffectiveDate.HasValue) ? priceListCode.EffectiveDate.Value : DateTime.MinValue,
					EED = null
				});
			}

			foreach (var priceListService in convertedPriceList.PriceListServices)
			{

				if (IsEffectiveDateLessThanMinDate(minimumDate, priceListService.EffectiveDate))
					minimumDate = priceListService.EffectiveDate.Value;

				string zoneNameValue = priceListService.ZoneName != null ? priceListService.ZoneName.Trim() : null;

				importedZonesServicesList.Add(new ImportedZoneService()
				{
					ServiceId = priceListService.ZoneServiceConfigId,
					ZoneName = zoneNameValue,
					BED = priceListService.EffectiveDate.HasValue ? priceListService.EffectiveDate.Value : DateTime.MinValue,
					EED = null
				});
			}

			foreach (var priceListRate in convertedPriceList.PriceListRates)
			{

				if (IsEffectiveDateLessThanMinDate(minimumDate, priceListRate.EffectiveDate))
					minimumDate = priceListRate.EffectiveDate.Value;

				if (priceListRate.Rate == null)
					continue;

				string zoneNameValue = priceListRate.ZoneName != null ? priceListRate.ZoneName.Trim() : null;

				importedRatesList.Add(new ImportedRate()
				{
					ZoneName = zoneNameValue,
					Rate = priceListRate.Rate.Value,
					CurrencyId = currencyId,
					BED = (priceListRate.EffectiveDate.HasValue) ? priceListRate.EffectiveDate.Value : DateTime.MinValue,
					EED = null,
				});
			}

			foreach (KeyValuePair<int, List<PriceListRate>> item in convertedPriceList.PriceListOtherRates)
			{
				foreach (PriceListRate priceListRate in item.Value)
				{

					if (IsEffectiveDateLessThanMinDate(minimumDate, priceListRate.EffectiveDate))
						minimumDate = priceListRate.EffectiveDate.Value;

					if (priceListRate.Rate == null)
						continue;

					string zoneNameValue = priceListRate.ZoneName != null ? priceListRate.ZoneName.Trim() : null;

					importedRatesList.Add(new ImportedRate()
					{
						ZoneName = zoneNameValue,
						Rate = priceListRate.Rate.Value,
						CurrencyId = currencyId,
						RateTypeId = item.Key,
						BED = (priceListRate.EffectiveDate.HasValue) ? priceListRate.EffectiveDate.Value : DateTime.MinValue,
						EED = null,
					});
				}
			}



			#region Imported Rates Validation

			//TODO: this logic needs to be removed when handling rates validation is done in business rules. 
			//For example when a rule has an action that takes the rate with min date or the rate with min value. At this case, the below logic is not needed.
			//var importedRatesByZoneName = new Dictionary<string, List<ImportedRate>>();

			//foreach (var importedRate in importedRatesList.OrderBy(x => x.BED))
			//{
			//    if (importedRate.ZoneName == null)
			//        continue;

			//    List<ImportedRate> zoneRates;
			//    if (!importedRatesByZoneName.TryGetValue(importedRate.ZoneName, out zoneRates))
			//    {
			//        zoneRates = new List<ImportedRate>();
			//        importedRatesByZoneName.Add(importedRate.ZoneName, zoneRates);
			//    }

			//    zoneRates.Add(importedRate);
			//}

			//var validatedListofImportedRates = new List<ImportedRate>();

			//foreach (var zoneImportedRates in importedRatesByZoneName.Values)
			//{
			//    bool allRatesWithSameValue = !zoneImportedRates.Select(item => item.Rate)
			//          .Distinct()
			//          .Skip(1)
			//          .Any();

			//    if (allRatesWithSameValue)
			//        validatedListofImportedRates.Add(zoneImportedRates.First()); //The rate with min date
			//    else
			//        validatedListofImportedRates.AddRange(zoneImportedRates); //Add them all to catch them in business rules
			//}

			#endregion

			ImportedCodes.Set(context, importedCodesList);
			AllImportedCodes.Set(context, new AllImportedCodes() { ImportedCodes = importedCodesList });
			FilteredImportedCodes.Set(context, convertedPriceList.FilteredPriceListCodes);
			ImportedZonesServices.Set(context, importedZonesServicesList);
			ImportedRates.Set(context, importedRatesList);
			ImportedRateTypeIds.Set(context, convertedPriceList.PriceListOtherRates.Keys);
			ImportedServiceTypeIds.Set(context, convertedPriceList.PriceListServices.Select(item => item.ZoneServiceConfigId).Distinct());
			IncludeServices.Set(context, convertedPriceList.IncludeServices);
			MinimumDate.Set(context, minimumDate);

			TimeSpan spent = DateTime.Now.Subtract(startReading);
			context.WriteTrackingMessage(LogEntryType.Information, "Finished reading {0} records from the excel file. It took: {1}.", convertedPriceList.PriceListCodes.Count, spent);
		}

		#region Private Methods

		private bool IsEffectiveDateLessThanMinDate(DateTime minimumDate, DateTime? effectiveDate)
		{
			return effectiveDate != null && (minimumDate == DateTime.MinValue || effectiveDate < minimumDate);
		}



		#endregion
	}
}
