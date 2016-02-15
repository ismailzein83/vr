using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Entities;

namespace Vanrise.BI.Data
{
    public interface IDimensionDataManager : IDataManager
    {
        List<DimensionInfo> GetDimensionInfo(string entityName);
        List<BIConfiguration<BIConfigurationEntity>> EntityDefinitions { set; }   
    }
}
