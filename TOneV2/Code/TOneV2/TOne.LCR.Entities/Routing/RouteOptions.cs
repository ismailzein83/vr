using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    [Serializable]
    public class RouteOptions
    {
        static RouteOptions()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RouteOptions),
                "IsBlock", "SupplierOptions");
        }
        public bool IsBlock { get; set; }

        public List<RouteSupplierOption> SupplierOptions { get; set; }
    }
}
