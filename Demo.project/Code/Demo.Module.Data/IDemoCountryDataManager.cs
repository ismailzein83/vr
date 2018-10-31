using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IDemoCountryDataManager : IDataManager
    {
        bool AreDemoCountriesUpdated(ref object updateHandle);

        List<DemoCountry> GetDemoCountries();

        bool Insert(DemoCountry demoCountry, out int insertedId);

        bool Update(DemoCountry demoCountry);

        bool Delete(int Id);
    }
}
