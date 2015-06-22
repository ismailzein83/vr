using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class ViewDataManager : BaseSQLDataManager, IViewDataManager
    {
        public ViewDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

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
                RequiredPermissions = this.ParseRequiredPermissionsString(GetReaderValue<string>(reader, "RequiredPermissions")),
                Audience = ((reader["Audience"] as string) != null) ? Common.Serializer.Deserialize<AudienceWrapper>(reader["Audience"] as string) : null
            };
            return view;
        }

        private Dictionary<string, List<string>> ParseRequiredPermissionsString(string value)
        {
            Dictionary<string, List<string>> requiredPermissions = null;

            if(value != null)
            {
                requiredPermissions = new Dictionary<string, List<string>>();

                string[] arrayOfPermissions = value.Split('|');

                foreach (string permission in arrayOfPermissions)
                {
                    string[] keyValuesArray = permission.Split(':');
                    List<string> flags = new List<string>();
                    foreach (string flag in keyValuesArray[1].Split(','))
                    {
                        flags.Add(flag.Trim());
                    }

                    requiredPermissions.Add(keyValuesArray[0].Trim(), flags);
                }
            }

            return requiredPermissions;
        }
    }


}
