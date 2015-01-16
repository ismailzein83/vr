
namespace TABS.Addons.PriceListExport
{
    public interface IRateSheetGenerator
    {
        byte[] GetPricelistWorkbook(TABS.PriceList pricelist,CodeView c);
    }
}
