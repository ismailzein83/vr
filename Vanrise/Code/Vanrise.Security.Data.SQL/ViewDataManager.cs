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

     

        public List<View> GetDynamicPages()
        {

            return GetItemsSP("sec.sp_View_GetByType", ViewMapper, ViewType.Dynamic);
            
        }
        public Vanrise.Entities.BigResult<View> GetFilteredDynamicViews(Vanrise.Entities.DataRetrievalInput<string> filter)
        {

            Dictionary<string,string> mapper=new Dictionary<string,string>();
           
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("sec.sp_View_CreateTempByFiltered", tempTableName, filter.Query, ViewType.Dynamic);
            };
            return RetrieveData(filter, createTempTableAction, ViewMapper, mapper);

        }


        public bool AddView(View view, out int insertedId)
        {
            string serialziedContent = null;
            if (view.ViewContent.BodyContents.Count > 0 || view.ViewContent.SummaryContents.Count > 0)
                serialziedContent = Common.Serializer.Serialize(view.ViewContent, true);
            string serialziedAudience = null;
            if (view.Audience.Groups.Count > 0 || view.Audience.Users.Count > 0)
                serialziedAudience = Common.Serializer.Serialize(view.Audience, true);
            object viewId;
            string url = "#/viewwithparams/Security/Views/DynamicPages/DynamicPagePreview";
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_Insert", out viewId, view.Name, view.Name, url, view.ModuleId, null,
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
            return GetItemSP("sec.sp_View_GetById ", ViewMapper, viewId);
        }
        private View ViewMapper(IDataReader reader)
        {

            View instance = new View
            {
                ViewId = (int)reader["Id"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Url = reader["Url"] as string,
                ModuleId = (int) reader["Module"],
                ModuleName = reader["ModuleName"] as string,
                RequiredPermissions = GetReaderValue<string>(reader, "RequiredPermissions"),
                Audience = ((reader["Audience"] as string) != null) ? Common.Serializer.Deserialize<AudienceWrapper>(reader["Audience"] as string) : null,
                ViewContent = ((reader["Content"] as string) != null) ? Common.Serializer.Deserialize<ViewContent>(reader["Content"] as string) : null,
                Type=(ViewType) reader["Type"],

            };
        

            return instance;
        }
        public bool UpdateViewRank(int viewId, int rank)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_UpdateRank", viewId, rank);
            return (recordesEffected > 0);
        }
       


    }


}
