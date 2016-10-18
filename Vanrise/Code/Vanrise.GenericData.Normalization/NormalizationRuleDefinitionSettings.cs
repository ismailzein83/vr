using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Normalization
{
    public class NormalizationRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("d2ba0e63-a47d-4f41-93c9-2df105edc26c"); }
        }
    }
}
