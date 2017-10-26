using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CompanyPricelistSettings : BaseCompanyExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("48AB58C0-8D37-4F6A-A5EC-5EAFE9AAA586"); } }
        public PricelistSettings PricelistSettings { get; set; }

    }
}
