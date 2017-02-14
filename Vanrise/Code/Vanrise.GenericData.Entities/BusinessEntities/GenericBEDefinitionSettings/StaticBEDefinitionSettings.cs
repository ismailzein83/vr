using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class StaticBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("84D9BD7C-DE09-43EA-BD25-85569667AADF"); }
        }
    }
}
