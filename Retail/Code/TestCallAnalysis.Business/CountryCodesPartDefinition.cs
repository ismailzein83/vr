using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;

namespace TestCallAnalysis.Business
{
    public class CountryCodesPartDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("51D18FDD-336C-4713-8258-304149B775FC"); } }

        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            throw new NotImplementedException();
        }
    }
}
