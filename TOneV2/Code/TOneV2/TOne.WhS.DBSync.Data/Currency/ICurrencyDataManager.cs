using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data
{
    public interface ICurrencyDataManager : IDataManager
    {
        void MigrateCurrenciesToDB(List<Currency> currencies);
    }
}
