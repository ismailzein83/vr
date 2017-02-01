using System;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Business
{
    public class VRRetailBEVisibilityEditorRuntime : Vanrise.Entities.VRModuleVisibilityEditorRuntime
    {
        public Dictionary<Guid, string> AccountBEDefinitionNamesById { get; set; }
    }

}
