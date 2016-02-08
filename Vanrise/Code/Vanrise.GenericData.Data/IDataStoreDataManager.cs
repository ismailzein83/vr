using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public interface IDataStoreDataManager : IDataManager
    {
        IEnumerable<DataStore> GetDataStores();

        bool AreDataStoresUpdated(ref object updateHandle);
    }
}
