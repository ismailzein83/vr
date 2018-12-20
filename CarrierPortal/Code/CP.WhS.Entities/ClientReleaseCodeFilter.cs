using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;

namespace CP.WhS.Entities
{
    public class ClientReleaseCodeFilter
    {
        public List<ReleaseCodeDimension> Dimession { get; set; }
        public List<int> SwitchIds { get; set; }
        public List<int> CustomerIds { get; set; }
        public List<int> SupplierIds { get; set; }
        public List<int> CountryIds { get; set; }
        public List<int> MasterSaleZoneIds { get; set; }
        public List<string> ColumnsToShow { get; set; }
        public AccountViewType AccountType { get; set; }
    }
}
