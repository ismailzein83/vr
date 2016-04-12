using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public interface ICode : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget
    {
        string Code { get; set; }

    }
}
