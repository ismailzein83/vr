using System;
using System.Data;

namespace TABS.Addons.TargetAnalysisExport
{
    public interface ISupplierTargetAnalysisSheetGenerator
    {
        byte[] GetSupplierTargetAnalysisWorkbook(DataTable DataSource, DateTime fromDate, DateTime toDate, bool WithAvreges);
    }
}
