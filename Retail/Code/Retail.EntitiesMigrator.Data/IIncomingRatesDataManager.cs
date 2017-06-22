using System.Collections.Generic;
using Retail.EntitiesMigrator.Entities;

namespace Retail.EntitiesMigrator.Data
{
    public interface IIncomingRatesDataManager : IDataManager
    {
        IEnumerable<IncomingRate> GetIncomingRates();
    }
}
