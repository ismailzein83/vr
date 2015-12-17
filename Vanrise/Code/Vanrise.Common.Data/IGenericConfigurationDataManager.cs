using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IGenericConfigurationDataManager:IDataManager
    {
        bool UpdateConfiguration(string ownerKey, int typeId,GenericConfiguration genericConfig);
        Dictionary<string, GenericConfiguration> GetALllConfigurations();
        bool AreGenericConfigurationsUpdated(ref object updateHandle);
    }
}
