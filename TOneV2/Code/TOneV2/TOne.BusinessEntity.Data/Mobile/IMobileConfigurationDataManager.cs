using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IMobileConfigurationDataManager : IDataManager
    {
        bool AddMobileConfiguration(MobileConfiguration configuration, out int insertedId);

        bool UpdateMobileConfiguration(MobileConfiguration configuration);

        IEnumerable<MobileConfiguration> GetConfigurations(int? configId);
    }
}
