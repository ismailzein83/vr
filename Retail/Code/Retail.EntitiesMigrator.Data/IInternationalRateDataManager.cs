using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.EntitiesMigrator.Entities;

namespace Retail.EntitiesMigrator.Data
{
    public interface IInternationalRateDataManager : IDataManager
    {
        IEnumerable<InternationalRate> GetInternationalRates();
    }
}
