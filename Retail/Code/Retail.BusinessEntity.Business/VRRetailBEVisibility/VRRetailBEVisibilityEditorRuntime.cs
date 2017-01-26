using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class VRRetailBEVisibilityEditorRuntime : Vanrise.Entities.VRModuleVisibilityEditorRuntime
    {
        public Dictionary<Guid, string> AccountBEDefinitionNamesById { get; set; }
    }
}
