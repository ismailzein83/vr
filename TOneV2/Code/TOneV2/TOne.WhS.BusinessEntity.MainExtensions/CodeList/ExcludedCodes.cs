using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CodeList
{
    public class ExcludedCodes : ExcludedDestinations
    {
        public override Guid ConfigId { get { return new Guid("AA1911DF-E45F-4CF6-BCA5-ECDFDD246FEE"); } }

        public List<string> Codes { get; set; }
    }
}
