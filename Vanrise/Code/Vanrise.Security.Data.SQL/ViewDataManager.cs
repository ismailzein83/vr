using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Security.Data.SQL
{
    public class ViewDataManager : BaseSQLDataManager, IViewDataManager
    {
        public List<Entities.View> GetViews()
        {
            return GetItemsSP("sec.sp_View_GetAll", ViewMapper);
        }

        Entities.View ViewMapper(IDataReader reader)
        {
            Entities.View view = new Entities.View
            {
                ViewId = (int)reader["Id"],
                Name = reader["Name"] as string,
                Url = reader["Url"] as string,
                ModuleId = (int) reader["Module"],
                RequiredPermissions = ((reader["RequiredPermissions"] as string) != null) ? Common.Serializer.Deserialize<Dictionary<string, List<string>>>(reader["RequiredPermissions"] as string) : null
            };
            return view;
        }
    }


}
