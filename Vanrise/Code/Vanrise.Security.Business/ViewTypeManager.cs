using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class ViewTypeManager
    {
        public IEnumerable <ViewType> GetViewTypes()
        {
            return GetViewTypeConfigs();
        }
        public Guid GetViewTypeIdByName(string name)
        {
            if (name == null)
                throw new NullReferenceException("ViewTypeName");
            var viewTypes = GetViewTypeConfigs();
            var viewType = viewTypes.FirstOrDefault(x=>x.Name == name);
            if(viewType == null)
                 throw new NullReferenceException("ViewType");
            return viewType.ExtensionConfigurationId;
        }
        
        public IEnumerable<ViewType> GetViewTypeConfigs()
        {
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<ViewType>(ViewType.EXTENSION_TYPE);
        }

    }
}
