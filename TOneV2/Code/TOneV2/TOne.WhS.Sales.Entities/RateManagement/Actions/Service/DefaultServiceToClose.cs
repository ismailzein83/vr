using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultServiceToClose
    {
        public DateTime CloseEffectiveDate { get; set; }
        private List<ExistingDefaultService> _changedExistingDefaultServices = new List<ExistingDefaultService>();
        public List<ExistingDefaultService> ChangedExistingDefaultServices
        {
            get
            {
                return _changedExistingDefaultServices;
            }
        }
    }
}
