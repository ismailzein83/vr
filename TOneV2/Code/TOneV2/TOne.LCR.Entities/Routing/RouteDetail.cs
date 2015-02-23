using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    [Serializable]
    public class RouteDetail
    {
        public string CustomerID { get; set; }

        public string Code { get; set; }

        public int SaleZoneId { get; set; }

        public decimal Rate { get; set; }

        public short ServicesFlag { get; set; }

        public RouteOptions Options { get; set; }

        public RouteDetail Clone()
        {
            return CloneHelper.Clone<RouteDetail>(this);
            //RouteDetail r = this.MemberwiseClone() as RouteDetail;
            //if(this.Options != null)
            //{
            //    r.Options = this.Options.Clone() as RouteOptions;
            //    if(this.Options.SupplierOptions != null)
            //    {
            //        foreach (var supOption in this.Options.SupplierOptions)
            //            r.Options.SupplierOptions.Add(supOption.Clone() as RouteSupplierOption);
            //    }
            //}
            //return r;
        }


    }
}