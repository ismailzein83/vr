using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.Voice.Business
{
    public class InternationalIdentificationManager
    {
        public IEnumerable<InternationalIdentificationTemplate> GetInternationalIdentificationTemplates()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<InternationalIdentificationTemplate>(InternationalIdentificationTemplate.EXTENSION_TYPE);
        }
    }
}
