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
    public class SpecialityDataManager : BaseSQLDataManager, ISpecialityDataManager
    {
        public SpecialityDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<Speciality> GetSpecialities()
        {
            return GetItemsSP("[dbo].[sp_Speciality_GetAll]", SpecialityMapper);
        }

       
        Speciality SpecialityMapper(IDataReader reader)
        {
            return new Speciality
            {
                SpecialityId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Title"),
            };
        }
    }
}
