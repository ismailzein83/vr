using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class TelesTemplateEntityDefinitionSettings : Vanrise.Entities.GenericLKUPDefinitionExtendedSettings
    {
        public static Guid s_configId { get { return new Guid("8C4D41EB-C137-48B2-8EAA-B5428734831D"); } }
        public override Guid ConfigId
        {
            get { return s_configId; }
        }
        public override string RuntimeEditor
        {
            get
            {
                return "retail-teles-telestemplateentitysettings";
            }
        }
    }
}
