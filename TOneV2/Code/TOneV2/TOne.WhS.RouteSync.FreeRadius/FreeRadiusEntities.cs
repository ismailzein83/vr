using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<CustomerMapping> CustomerMappings { get; set; }

        public List<SupplierMapping> SupplierMappings { get; set; }
    }

    public class CustomerMapping
    {
        public string Mapping { get; set; }
    }

    public class SupplierMapping
    {
        public string Mapping { get; set; }
    }

    public class FreeRadiusRouteOption
    {
        public string Option { get; set; }
        public decimal Percentage { get; set; }
    }

    public class FreeRadiusConvertedRouteOption
    {
        public string Option { get; set; }
        public decimal Min_perc { get; set; }
        public decimal Max_perc { get; set; }
    }

    public class FreeRadiusConvertedRoutesPayload
    {
        public FreeRadiusConvertedRoutesPayload()
        {
            this.ConvertedRoutesBuffer = new Dictionary<string, ConvertedRoutesByCodeWithoutLastDigit>();
        }

        /// <summary>
        /// Key: CustomerId
        /// </summary>
        public Dictionary<string, ConvertedRoutesByCodeWithoutLastDigit> ConvertedRoutesBuffer { get; set; }
    }

    public class ConvertedRoutesByCodeWithoutLastDigit : Dictionary<string, ConvertedRoutesByOption>
    {

    }

    public class ConvertedRoutesByOption : Dictionary<OptionWithPercentage, ConvertedRoutesWithCodeLastDigit>
    {

    }

    public class ConvertedRoutesWithCodeLastDigit
    {
        public List<ConvertedRouteWithCodeLastDigit> ConvertedRouteWithCodeLastDigitList { get; set; }

        public FreeRadiusConvertedRouteOption FreeRadiusConvertedRouteOption { get; set; }
    }

    public class ConvertedRouteWithCodeLastDigit
    {
        public FreeRadiusConvertedRoute FreeRadiusConvertedRoute { get; set; }

        public string CodeLastDigit { get; set; }
    }

    public struct CustomerCode
    {
        public string CustomerId { get; set; }
        public string Code { get; set; }
    }

    public struct OptionWithPercentage
    {
        public string Option { get; set; }
        public decimal Min_perc { get; set; }
        public decimal Max_perc { get; set; }

        public override int GetHashCode()
        {
            return this.Option.GetHashCode() + this.Min_perc.GetHashCode() + this.Max_perc.GetHashCode();
        }
    }

    public class FreeRadiusSwapTablePayload
    {
        public bool SyncSaleCodeZones { get; set; }
    }
}
