using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;
//using Vanrise.Entities;

namespace TOne.BusinessEntity.Data.SQL
{
    public class LookUpDataManager : BaseTOneDataManager, ILookUpDataManager
    {
        public List<Country> GetCountries()
        {

            return GetItemsSP("[BEntity].[sp_Countries_GetAll]", LookUpMapper<Country>);
        }
        public List<City> GetCities()
        {

            return GetItemsSP("[BEntity].[sp_Cities_GetAll]", LookUpMapper<City>);
        }
        T LookUpMapper<T>(IDataReader reader) where T : LookUpItem
        {
            T obj = Activator.CreateInstance<T>();
            obj.LookUpID = (int)reader["ID"];
            obj.Description = reader["Description"] as string;
            return obj;
        }
    }
}
