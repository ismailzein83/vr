using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;


namespace Vanrise.Common
{
    public interface IExtensionConfigurationManager : IBEManager
    {
        IEnumerable<Q> GetExtensionConfigurations<Q>(string type) where Q : ExtensionConfiguration;

        string GetExtensionConfigurationTitle<T>(Guid extensionConfigId, string type) where T : ExtensionConfiguration;
    }
}
