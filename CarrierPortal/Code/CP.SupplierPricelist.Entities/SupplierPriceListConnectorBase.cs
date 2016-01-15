using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Entities
{
    public abstract class SupplierPriceListConnectorBase
    {
        public int ConfigId { get; set; }
        public abstract InitiatePriceListOutput InitiatePriceList(IInitiateUploadContext context);
        public abstract PriceListProgressOutput GetPriceListProgressOutput(IPriceListProgressContext context);
    }
    public enum InitiateSupplierResult { Uploaded, Failed }
    public enum PriceListProgressResult { Completed, Failed }

    public class InitiatePriceListOutput
    {
        public InitiateSupplierResult Result { get; set; }

        public Object InitiateTestInformation { get; set; }

        public string FailureMessage { get; set; }
    }

    public interface IInitiateUploadContext
    {
        int UserId { get; }
        string PriceListType { get; }
        VRFile File { get; }
        DateTime EffectiveOnDateTime { get; }
    }
    public class PriceListProgressOutput
    {
        public PriceListProgressResult Result { get; set; }

        public Object TestProgress { get; set; }

        public PriceListResult PriceListResult { get; set; }
    }
    public interface IPriceListProgressContext
    {
        Object InitiateTestInformation { get; }

        Object RecentTestProgress { get; }
    }

}
