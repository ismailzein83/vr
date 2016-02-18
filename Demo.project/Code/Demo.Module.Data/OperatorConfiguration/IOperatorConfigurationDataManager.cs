using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface IOperatorConfigurationDataManager:IDataManager
    {
        List<OperatorConfiguration> GetOperatorConfigurations();
        bool Insert(OperatorConfiguration config, out int infoId);
        bool Update(OperatorConfiguration config);
        bool AreOperatorConfigurationsUpdated(ref object updateHandle);
    }
}
