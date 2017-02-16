using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class ImportedRowValidator
    {
        #region Fields / Constructors

        private IEnumerable<IImportedRowValidator> _validators;

        public ImportedRowValidator()
        {
            _validators = new List<IImportedRowValidator>()
			{
				new ZoneValidator(),
				new RateValidator(),
				new EffectiveDateValidator()
			};
        }

        #endregion

        public void ValidateImportedFile(ImportedFileValidationContext context)
        {
            if (context.ImportedRows == null || context.ImportedRows.Count() == 0)
                return;

            var importedZones = new Dictionary<string, ImportedZone>();
            var duplicatedZones = new Dictionary<string, ImportedZone>();

            for (int i = 0; i < context.ImportedRows.Count(); i++)
            {
                ImportedRow importedRow = context.ImportedRows.ElementAt(i);

                if (importedRow.Zone == null)
                    continue;

                string zoneName = importedRow.Zone.ToLower();

                ImportedZone importedZone;

                if (!importedZones.TryGetValue(zoneName, out importedZone))
                {
                    importedZone = new ImportedZone() { ZoneName = zoneName };
                    importedZones.Add(zoneName, importedZone);
                }

                importedZone.RowIndexes.Add(i);
                importedZone.ImportedRows.Add(importedRow);

                if (importedZone.RowIndexes.Count > 1)
                {
                    if (!duplicatedZones.ContainsKey(zoneName))
                    {
                        duplicatedZones.Add(zoneName, importedZone);
                    }
                }
            }

            var invalidImportedRows = new List<InvalidImportedRow>();

            foreach (ImportedZone duplicatedZone in duplicatedZones.Values)
            {
                for (int i = 0; i < duplicatedZone.ImportedRows.Count; i++)
                {
                    ImportedRow importedRow = duplicatedZone.ImportedRows.ElementAt(i);
                    invalidImportedRows.Add(new InvalidImportedRow()
                    {
                        RowIndex = duplicatedZone.RowIndexes.ElementAt(i),
                        ZoneId = GetSaleZoneId(context.SaleZonesByZoneName, duplicatedZone.ZoneName),
                        ImportedRow = importedRow,
                        ErrorMessage = string.Format("Zone '{0}' is duplicated", importedRow.Zone)
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
                    ImportedRow = context.ImportedRow,
                    ZoneDraft = context.ZoneDraft,
                    ExistingZone = context.ExistingZone,
                    CountryBEDsByCountry = context.CountryBEDsByCountry
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
                    context.ErrorMessage = string.Join(";", errorMessages);
                return false;
            }
        }

        #region Private Members

        private class ImportedZone
        {
            public ImportedZone()
            {
                RowIndexes = new List<int>();
                ImportedRows = new List<ImportedRow>();
            }

            public string ZoneName { get; set; }

            public List<int> RowIndexes { get; set; }

            public List<ImportedRow> ImportedRows { get; set; }
        }

        private long? GetSaleZoneId(Dictionary<string, SaleZone> saleZonesByZoneName, string saleZoneName)
        {
            SaleZone saleZone = saleZonesByZoneName.GetRecord(saleZoneName);
            if (saleZone == null)
                return null;
            return saleZone.SaleZoneId;
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

            if (string.IsNullOrEmpty(rateValue))
            {
                context.ErrorMessage = "Rate is empty";
                return false;
            }

            decimal rateAsDecimal;
            if (!decimal.TryParse(rateValue, out rateAsDecimal))
            {
                context.ErrorMessage = "Rate is invalid";
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

            DateTime effectiveDateAsDateTime;
            if (!DateTime.TryParse(effectiveDate, out effectiveDateAsDateTime))
            {
                context.ErrorMessage = "Effective date is invalid";
                return false;
            }

            if (context.ExistingZone != null)
            {
                if (context.ExistingZone.BED > effectiveDateAsDateTime)
                {
                    context.ErrorMessage = string.Format("Effective date is smaller than the Zone's BED '{0}'", context.ExistingZone.BED);
                    return false;
                }

                if (context.ExistingZone.EED.HasValue)
                {
                    context.ErrorMessage = string.Format("Zone '{0}' will be closed on '{1}'. Cannot define new Rates for pending closed Zones", context.ExistingZone.SaleZoneId, context.ExistingZone.EED);
                    return false;
                }

                if (context.OwnerType == BusinessEntity.Entities.SalePriceListOwnerType.Customer)
                {
                    DateTime countryBED;
                    if (!context.CountryBEDsByCountry.TryGetValue(context.ExistingZone.CountryId, out countryBED))
                    {
                        context.ErrorMessage = string.Format("Country '{0}' of Zone '{1}' is not sold to the Customer", context.ExistingZone.CountryId, context.ExistingZone.SaleZoneId);
                        return false;
                    }

                    if (countryBED > effectiveDateAsDateTime)
                    {
                        context.ErrorMessage = string.Format("Country '{0}' is sold to the Customer on '{1}' which is greater than the effective date '{2}'", context.ExistingZone.CountryId, countryBED, effectiveDateAsDateTime);
                        return false;
                    }
                }
            }

            context.ErrorMessage = null;
            return true;
        }
    }

    #endregion
}
