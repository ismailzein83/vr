using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RateManagement.Actions.Country
{
    public class AllCustomerCountriesToChange : Vanrise.BusinessProcess.Entities.IRuleTarget
    {
        public IEnumerable<CustomerCountryToChange> CustomerCountriesToChange { get; set; }
        public object Key
        {
            get { return null; }
        }
        public string TargetType
        {
            get { return "AllCustomerCountriesToChange"; }
        }
    }
}
