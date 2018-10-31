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
    public class DesksizeDataManager : BaseSQLDataManager, IDesksizeDataManager
    {
        public DesksizeDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<Desksize> GetDesksizes()
        {
            return GetItemsSP("[dbo].[sp_Desksize_GetAll]", DesksizeMapper);
        }


        Desksize DesksizeMapper(IDataReader reader)
        {
            return new Desksize
            {
                DesksizeId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Size"),
            };
        }
    }
}
