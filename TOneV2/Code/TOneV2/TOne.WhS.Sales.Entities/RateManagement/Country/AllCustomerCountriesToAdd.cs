using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class AllCustomerCountriesToAdd : Vanrise.BusinessProcess.Entities.IRuleTarget
    {
        public IEnumerable<CustomerCountryToAdd> CustomerCountriesToAdd { get; set; }
        public object Key
        {
            get { return null; }
        }
        public string TargetType
        {
            get { return "AllCustomerCountriesToAdd"; }
        }
    }
}
