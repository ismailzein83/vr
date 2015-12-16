using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public abstract class GenericConfigurationManager
    {
        public void UpdateConfiguration(GenericConfiguration genericConfig)
        {
            string ownerKey = genericConfig.OwnerKey;
            if (ownerKey == null)
                ownerKey = Guid.Empty.ToString();
            int typeId = TypeManager.Instance.GetTypeId(this.GetType());
        }

        public GenericConfiguration GetConfiguration(string ownerKey)
        {
            if (ownerKey == null)
                ownerKey = Guid.Empty.ToString();
            int typeId = TypeManager.Instance.GetTypeId(this.GetType());
            return null;
        }
    }
}
