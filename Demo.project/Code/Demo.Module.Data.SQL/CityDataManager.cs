using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    class CityDataManager : BaseSQLDataManager, ICityDataManager
    {
        public CityDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {

        }

        public List<Entities.City> GetCities()
        {
            return GetItemsSP("[dbo].[sp_City_GetAll]", CityMapper);
        }

        public bool Update(Entities.City city)
        {
            int recordsEffected = ExecuteNonQuerySP("[dbo].[sp_City_Update]", city.Id, city.Name);
            return (recordsEffected > 0);
        }

        public bool Insert(Entities.City city, out int insertedId)
        {
            object Id;
            int recordsEffected = ExecuteNonQuerySP("[dbo].[sp_City_Insert]", out Id, city.Name);
            insertedId = (int)Id;
            return (recordsEffected > 0);
        }

        public bool GetCitie(int Id, out string Name)
        {
            object CityName;
            int recordsEffected = ExecuteNonQuerySP("[dbo].[sp_City_GetCity]", out CityName, Id);
            Name = (string)CityName;
            return (recordsEffected > 0);
        }


        City CityMapper(IDataReader reader)
        {
            City city = new City();
            city.Id = (int)reader["ID"];
            city.Name = reader["Name"] as string;
            
            return city;
        }


        
    }
}
