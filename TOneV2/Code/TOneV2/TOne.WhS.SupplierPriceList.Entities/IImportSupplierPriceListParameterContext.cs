using System;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public interface IImportSPLContext
    {
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
