using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface ICurrencyExchangeRateWithEEDDataManager : IDataManager
    {
        string ConnectionString { set; }

        string ConnectionStringName { set; }

        void ApplyExchangeRateWithEESInDB(List<Vanrise.Entities.ExchangeRateWithEED> exchangeRates);
    }
}
