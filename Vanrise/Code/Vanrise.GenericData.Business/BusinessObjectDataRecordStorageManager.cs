using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BusinessObjectDataRecordStorageManager
    {

        public IEnumerable<BusinessObjectDataRecordStorageConfig> GetDataRecordStorageTemplateConfigs()
        {
            return Vanrise.Common.BusinessManagerFactory.GetManager<IExtensionConfigurationManager>().GetExtensionConfigurations<BusinessObjectDataRecordStorageConfig>(BusinessObjectDataRecordStorageConfig.EXTENSION_TYPE);
        }
    }
}
