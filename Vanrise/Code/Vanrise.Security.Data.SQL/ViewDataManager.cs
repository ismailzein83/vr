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
                Audience = ((reader["Audience"] as string) != null) ? Common.Serializer.Deserialize<AudienceWrapper>(reader["Audience"] as string) : null,
                Type=(ViewType) reader["Type"]

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


        public List<View> GetDynamicPages()
        {

            return GetItemsSP("sec.sp_View_GetByType", DynamicPageMapper, ViewType.Dynamic);
        }
        public List<View> GetFilteredDynamicViews(string filter)
        {

            return GetItemsSP("sec.sp_View_GetFiltered", DynamicPageMapper, filter,ViewType.Dynamic);
        }
        private View DynamicPageMapper(IDataReader reader)
        {
            View instance = new View
            {
                // ID = (int)reader["ID"],
                ViewId= (int)reader["Id"],
                Name = reader["PageName"] as string,
                Url = reader["Url"] as string,
                ModuleName = reader["ModuleName"] as string,
                ModuleId = (int)reader["ModuleId"],
                Audience = ((reader["Audience"] as string) != null) ? Common.Serializer.Deserialize<AudienceWrapper>(reader["Audience"] as string) : null,
                Type = (ViewType)reader["Type"],
                ViewContent = Common.Serializer.Deserialize<ViewContent>(reader["Content"] as string),
            };
            return instance;
        }

        public bool SaveView(View view, out int insertedId)
        {
            string serialziedContent = null;
            if (view.ViewContent.BodyContents.Count > 0 || view.ViewContent.SummaryContents.Count > 0)
                serialziedContent = Common.Serializer.Serialize(view.ViewContent, true);
            string serialziedAudience = null;
            if (view.Audience.Groups.Count > 0 || view.Audience.Users.Count > 0)
                serialziedAudience = Common.Serializer.Serialize(view.Audience, true);
            object viewId;
            string url = "#/viewwithparams/Security/Views/DynamicPages/DynamicPagePreview";
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_Insert", out viewId, view.Name, url, view.ModuleId, null,
               serialziedAudience, serialziedContent, ViewType.Dynamic);
            insertedId  = (recordesEffected > 0) ? (int)viewId : -1;
            return (recordesEffected > 0);
            //  return false;
        }
        public bool UpdateView(View view)
        {
            string serialziedContent = null;
            if (view.ViewContent.BodyContents.Count > 0 || view.ViewContent.SummaryContents.Count > 0)
                serialziedContent = Common.Serializer.Serialize(view.ViewContent, true);
            string serialziedAudience = null;
            if (view.Audience.Groups.Count > 0 || view.Audience.Users.Count > 0)
                serialziedAudience = Common.Serializer.Serialize(view.Audience, true);
            string url = "#/viewwithparams/Security/Views/DynamicPages/DynamicPagePreview";
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_Update", view.ViewId, view.Name, url, view.ModuleId, null,
               serialziedAudience, serialziedContent, ViewType.Dynamic);
            return (recordesEffected > 0);
            //  return false;
        }
        public bool DeleteView(int viewId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_Delete", viewId);
            return (recordesEffected > 0);
        }

        public View GetView(int viewId)
        {
            return GetItemSP("sec.sp_View_GetById ", GetViewMapper, viewId);
        }
        private View GetViewMapper(IDataReader reader)
        {

            View instance = new View
            {
                ViewId = (int)reader["Id"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Url = reader["Url"] as string,
                ModuleId = (int) reader["Module"],
                RequiredPermissions = this.ParseRequiredPermissionsString(GetReaderValue<string>(reader, "RequiredPermissions")),
                Audience = ((reader["Audience"] as string) != null) ? Common.Serializer.Deserialize<AudienceWrapper>(reader["Audience"] as string) : null,
                ViewContent = Common.Serializer.Deserialize<ViewContent>(reader["Content"] as string),
                Type=(ViewType) reader["Type"],
                

            };
        

            return instance;
        }


    }


}
