using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using TestCallAnalysis.Entities;

namespace TestCallAnalysis.Business
{
    public class CountryCodesPartSettings : AccountPartSettings
    {
        public override Guid ConfigId { get { return new Guid("E1174D46-B8C9-46D1-A993-F1E14565C551"); } }
        public List<CountryCode> CountryCodes { get; set; }
    }
}
