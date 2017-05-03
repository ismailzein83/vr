using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BEDefinitionOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("8DC29F02-7197-4C60-8E21-CBDE0C2AE87B"); }
        }

        public Guid BusinessEntityDefinitionId { get; set; }

        public string OverriddenTitle { get; set; }

        public BusinessEntityDefinitionSettings OverriddenSettings { get; set; }
    }
}
