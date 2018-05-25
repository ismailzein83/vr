using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business
{
	public class ImportedRowValidator
	{
		#region Fields / Constructors

		private IEnumerable<IImportedRowValidator> _validators;
		private IEnumerable<int> _allRateTypeIds;

		public ImportedRowValidator()
		{
			_validators = new List<IImportedRowValidator>()
				  {
				new CountryValidator(),
						new ZoneValidator(),
						new RateValidator(),
						new EffectiveDateValidator(),
						new OtherRateValidator()
				  };
			var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();
			_allRateTypeIds = rateTypeManager.GetAllRateTypes().Select(item => item.RateTypeId);
		}

		#endregion

		public void ValidateImportedFile(ImportedFileValidationContext context)
		{
			if (context.ImportedRows == null || context.ImportedRows.Count() == 0)
				return;

			var importedRowsByZoneName = new Dictionary<string, List<ImportedRowWrapper>>();
			var duplicateZoneNameKeys = new HashSet<string>();

			for (int i = 0; i < context.ImportedRows.Count(); i++)
			{
				ImportedRow importedRow = context.ImportedRows.ElementAt(i);

				if (string.IsNullOrWhiteSpace(importedRow.Zone))
					continue;

				List<ImportedRowWrapper> zoneImportedRows;
				string zoneNameKey = BulkActionUtilities.GetZoneNameKey(importedRow.Zone);

				if (!importedRowsByZoneName.TryGetValue(zoneNameKey, out zoneImportedRows))
				{
					zoneImportedRows = new List<ImportedRowWrapper>();
					importedRowsByZoneName.Add(zoneNameKey, zoneImportedRows);
				}
				else
				{
					duplicateZoneNameKeys.Add(zoneNameKey);
				}

				zoneImportedRows.Add(new ImportedRowWrapper() { RowIndex = i, ImportedRow = importedRow });
			}

			var invalidImportedRows = new List<InvalidImportedRow>();

			foreach (string duplicateZoneNameKey in duplicateZoneNameKeys)
			{
				long? importedZoneId = GetSaleZoneId(context.SaleZonesByZoneName, duplicateZoneNameKey);
				IEnumerable<ImportedRowWrapper> zoneImportedRows = importedRowsByZoneName.GetRecord(duplicateZoneNameKey);

				foreach (ImportedRowWrapper zoneImportedRow in zoneImportedRows)
				{
					invalidImportedRows.Add(new InvalidImportedRow()
					{
						RowIndex = zoneImportedRow.RowIndex,
						ZoneId = importedZoneId,
						ImportedRow = zoneImportedRow.ImportedRow,
						ErrorMessage = string.Format("Zone '{0}' is duplicated", zoneImportedRow.ImportedRow.Zone)
					});
				}
			}

			context.InvalidImportedRows = invalidImportedRows;
		}

		public bool IsImportedRowValid(IIsImportedRowValidContext context)
		{
			bool isValid = true;
			var errorMessages = new List<string>();

			foreach (IImportedRowValidator validator in _validators)
			{
				var isValidContext = new IsValidContext()
				{
					OwnerType = context.OwnerType,
					OwnerId = context.OwnerId,
					AllRateTypeIds = _allRateTypeIds,
					ImportedRow = context.ImportedRow,
					ZoneDraft = context.ZoneDraft,
					ExistingZone = context.ExistingZone,
					CountryBEDsByCountry = context.CountryBEDsByCountry,
					ClosedCountryIds = context.ClosedCountryIds,
					DateTimeFormat = context.DateTimeFormat,
					AdditionalCountryBEDsByCountryId = context.AdditionalCountryBEDsByCountryId
				};

				if (!validator.IsValid(isValidContext))
				{
					if (isValidContext.ErrorMessage != null)
						errorMessages.Add(isValidContext.ErrorMessage);
					isValid = false;
				}
			}

			if (isValid)
			{
				context.Status = ImportedRowStatus.Valid;
				context.ErrorMessage = null;
				return true;
			}
			else
			{
				context.Status = ImportedRowStatus.Invalid;
				if (errorMessages.Count > 0)
					context.ErrorMessage = string.Join(" ; ", errorMessages);
				return false;
			}
		}

		#region Private Members

		private long? GetSaleZoneId(Dictionary<string, SaleZone> saleZonesByZoneName, string saleZoneName)
		{
			SaleZone saleZone = saleZonesByZoneName.GetRecord(saleZoneName);
			if (saleZone == null)
				return null;
			return saleZone.SaleZoneId;
		}

		private class ImportedRowWrapper
		{
			public int RowIndex { get; set; }
			public ImportedRow ImportedRow { get; set; }
		}

		#endregion
	}

	#region Public Classes

	public class ImportedFileValidationContext
	{
		public IEnumerable<ImportedRow> ImportedRows { get; set; }

		public Dictionary<string, SaleZone> SaleZonesByZoneName { get; set; }

		public IEnumerable<InvalidImportedRow> InvalidImportedRows { get; set; }
	}

	public class CountryValidator : IImportedRowValidator
	{
		public bool IsValid(IIsValidContext context)
		{
			if (context.ExistingZone == null)
			{
				context.ErrorMessage = null;
				return true;
			}

			if (context.ClosedCountryIds.Contains(context.ExistingZone.CountryId))
			{
				context.ErrorMessage = "Country is closed";
				return false;
			}

			context.ErrorMessage = null;
			return true;
		}
	}

	public class ZoneValidator : IImportedRowValidator
	{
		public bool IsValid(IIsValidContext context)
		{
			string zoneName = context.ImportedRow.Zone;

			if (string.IsNullOrEmpty(zoneName))
			{
				context.ErrorMessage = "Zone is empty";
				return false;
			}

			if (context.ExistingZone == null)
			{
				context.ErrorMessage = "Zone does not exist";
				return false;
			}

			context.ErrorMessage = null;
			return true;
		}
	}

	public class RateValidator : IImportedRowValidator
	{
		public bool IsValid(IIsValidContext context)
		{
			string rateValue = context.ImportedRow.Rate;

			string errorMessage;
			if (!string.IsNullOrEmpty(rateValue) && !BulkActionUtilities.ValidateRateValue(rateValue, out errorMessage))
			{
				context.ErrorMessage = errorMessage;
				return false;
			}

			context.ErrorMessage = null;
			return true;
		}
	}

	public class EffectiveDateValidator : IImportedRowValidator
	{
		public bool IsValid(IIsValidContext context)
		{
			string effectiveDate = context.ImportedRow.EffectiveDate;

			if (string.IsNullOrEmpty(effectiveDate))
			{
				context.ErrorMessage = "Effective date is empty";
				return false;
			}

			string dateTimeFormat = context.DateTimeFormat;

			DateTime effectiveDateAsDateTime;
			if (!DateTime.TryParseExact(effectiveDate, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out effectiveDateAsDateTime))
			{
				context.ErrorMessage = string.Format("Effective date has wrong format (correct date format: '{0}')", dateTimeFormat);
				return false;
			}

			// Convert ImportedRow.EffectiveDate to a string in a format that the grid's date column expects
			context.ImportedRow.EffectiveDate = effectiveDateAsDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

			if (context.ExistingZone != null)
			{
				if (context.ExistingZone.BED > effectiveDateAsDateTime)
				{
					context.ErrorMessage = string.Format("Effective date is smaller than the Zone's BED '{0}'", UtilitiesManager.GetDateTimeAsString(context.ExistingZone.BED));
					return false;
				}

				if (context.ExistingZone.EED.HasValue)
				{
					context.ErrorMessage = string.Format("Zone '{0}' will be closed on '{1}'. Cannot define new Rates for pending closed Zones", context.ExistingZone.Name, UtilitiesManager.GetDateTimeAsString(context.ExistingZone.EED.Value));
					return false;
				}

				if (context.OwnerType == BusinessEntity.Entities.SalePriceListOwnerType.Customer)
				{
					DateTime countryBED;
					if (!context.CountryBEDsByCountry.TryGetValue(context.ExistingZone.CountryId, out countryBED))
					{
						DateTime newCountryBED;
						if (context.AdditionalCountryBEDsByCountryId.TryGetValue(context.ExistingZone.CountryId, out newCountryBED))
						{
							if (effectiveDateAsDateTime != newCountryBED)
							{
								context.ErrorMessage = string.Format("Country '{0}' is sold to the Customer on '{1}' which is different than the zone effective date '{2}'", context.ExistingZone.CountryId, UtilitiesManager.GetDateTimeAsString(newCountryBED), UtilitiesManager.GetDateTimeAsString(effectiveDateAsDateTime));
								return false;
							}
						}
						else
						{
							context.AdditionalCountryBEDsByCountryId.Add(context.ExistingZone.CountryId, effectiveDateAsDateTime);
						}
					}

					else if (countryBED > effectiveDateAsDateTime)
					{
						context.ErrorMessage = string.Format("Country '{0}' is sold to the Customer on '{1}' which is greater than the effective date '{2}'", context.ExistingZone.CountryId, UtilitiesManager.GetDateTimeAsString(countryBED), UtilitiesManager.GetDateTimeAsString(effectiveDateAsDateTime));
						return false;
					}
				}
			}

			context.ErrorMessage = null;
			return true;
		}
	}

	public class OtherRateValidator : IImportedRowValidator
	{
		public bool IsValid(IIsValidContext context)
		{
			context.ErrorMessage = null;
			IEnumerable<int> applicableRateTypeIds;
			bool isValid = true;
			List<string> errorMessages = new List<string>();
			List<int> validRateTypeIds = new List<int>();
			string errorMessage;

			if (context.ExistingZone == null)
				return true;

			if (context.OwnerType == SalePriceListOwnerType.SellingProduct)
				applicableRateTypeIds = context.AllRateTypeIds;
			else applicableRateTypeIds = BusinessEntity.Business.Helper.GetRateTypeIds(context.OwnerId, context.ExistingZone.SaleZoneId, DateTime.Now);

			foreach (var otherRate in context.ImportedRow.OtherRates)
			{
				if (string.IsNullOrEmpty(otherRate.Value))
					continue;

				if (!applicableRateTypeIds.Contains(otherRate.TypeId))
				{
					isValid = false;
					errorMessages.Add(string.Format("{0} Rate is not applicable for this zone.", otherRate.TypeName));
				}
				else if (!BulkActionUtilities.ValidateRateValue(otherRate.Value, out errorMessage))
				{
					isValid = false;
					errorMessages.Add(string.Join(" ", otherRate.TypeName, errorMessage));
				}
				else
				{
					validRateTypeIds.Add(otherRate.TypeId);
				}
			}

			if (!isValid)
			{
				context.ErrorMessage = string.Join("; ", errorMessages);
				return false;
			}

			context.ImportedRow.OtherRates = context.ImportedRow.OtherRates.FindAllRecords(item => validRateTypeIds.Contains(item.TypeId)).ToList();
			return true;

		}
	}
	#endregion
}
