using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class LookUpManager
    {
        ILookUpDataManager _dataManager;
        public LookUpManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ILookUpDataManager>();
        }
        public List<Country> GetCountries()
        {
            return _dataManager.GetCountries();
        }
        public List<City> GetCities()
        {
            return _dataManager.GetCities();
        }
    }
}
