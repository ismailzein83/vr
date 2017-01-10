using System.Linq;

namespace NP.IVSwitch.Entities
{
    public class CustomerRouteDetail
    {
        public ConvertedCustomerRoute Entity { get; set; }

        public string Options
        {
            get { return string.Join(",", Entity.Options.Select(r => r.ToString())); }
        }
    }
}
