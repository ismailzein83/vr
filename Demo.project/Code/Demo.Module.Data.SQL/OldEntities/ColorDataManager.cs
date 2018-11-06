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
    public class ColorDataManager : BaseSQLDataManager, IColorDataManager
    {
        public ColorDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<Color> GetColors()
        {
            return GetItemsSP("[dbo].[sp_Color_GetAll]", ColorMapper);
        }


        Color ColorMapper(IDataReader reader)
        {
            return new Color
            {
                ColorId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Color"),
                DesksizeId=GetReaderValue<int>(reader,"DesksizeId")
            };
        }
    }
}
