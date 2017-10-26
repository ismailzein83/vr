using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CompanyDefinitionPricelistSettings : BaseCompanyDefinitionExtendedSetting 
    {
        public override Guid ConfigId { get { return new Guid("48AB58C0-8D37-4F6A-A5EC-5EAFE9AAA586"); } }
        public override string RuntimeEditor{ get { return "vr-whs-be-pricelistsettings-editor"; } }

    }

    
}
