using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Entities;
namespace Vanrise.BI.Data
{
    public interface  IBIConfigurationDataManager : IDataManager
    {
        List<BIConfiguration<BIConfigurationMeasure>> GetMeasures();
        List<BIConfiguration<BIConfigurationEntity>> GetEntities();
    }
}
