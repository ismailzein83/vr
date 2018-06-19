using System;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public interface IImportSPLContext
    {
        SupplierPricelistType SupplierPricelistType { get; }
        int SupplierId { get; }
        TimeSpan CodeCloseDateOffset { get; }
        DateTime CodeEffectiveDate { get; }
        bool ProcessHasChanges { get; }
        int GetPriceListCurrencyId();
        void SetToTrueProcessHasChangesWithLock();
        int GetImportedRateCurrencyId(ImportedRate importedRate);
        decimal GetMaximumRateConverted(int currencyId);
        decimal MaximumRate { get; }
        DateTime RetroactiveDate { get; }
        string DateFormat { get; }
    }
}
