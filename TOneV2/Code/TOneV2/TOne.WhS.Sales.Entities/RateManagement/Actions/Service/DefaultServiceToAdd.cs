using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultServiceToAdd : Vanrise.Entities.IDateEffectiveSettings
    {
        public NewDefaultService NewDefaultService { get; set; }
        private List<ExistingDefaultService> _changedExistingDefaultServices = new List<ExistingDefaultService>();
        public List<ExistingDefaultService> ChangedExistingDefaultServices
        {
            get
            {
                return _changedExistingDefaultServices;
            }
        }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
