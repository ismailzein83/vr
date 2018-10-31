using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IDemoCurrencyDataManager : IDataManager
    {
        bool AreDemoCurrenciesUpdated(ref object updateHandle);

        List<DemoCurrency> GetDemoCurrencies();

        bool Insert(DemoCurrency demoCurrency, out int insertedId);

        bool Update(DemoCurrency DemoCurrency);

        bool Delete(int Id);
    }
}
