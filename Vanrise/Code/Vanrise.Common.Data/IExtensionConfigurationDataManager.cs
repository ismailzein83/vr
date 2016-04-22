using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IExtensionConfigurationDataManager:IDataManager
    {
        List<T> GetExtensionConfigurationsByType<T>(string type) where T : ExtensionConfiguration;
        bool AreExtensionConfigurationUpdated(string parameter, ref object updateHandle);
    }
}
