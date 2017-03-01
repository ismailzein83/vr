using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface ICityDataManager:IDataManager
    {
        List<City> GetCities();
        bool Update(City city);
        bool Insert(City city, out int insertedId);
        City GetCitie(int Id);
     
    }
}
