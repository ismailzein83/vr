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
        #region ctor
        public ViewDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public List<Entities.View> GetViews()
        {
            return GetItemsSP("sec.sp_View_GetAll", ViewMapper);
        }

        public bool AddView(View view)
        {
            string serialziedContent = null;
            if (view.ViewContent != null)
            {
                if ((view.ViewContent.BodyContents != null && view.ViewContent.BodyContents.Count > 0) || (view.ViewContent.SummaryContents != null && view.ViewContent.SummaryContents.Count > 0))
                    serialziedContent = Common.Serializer.Serialize(view.ViewContent, true);
            }
            string serialziedAudience = null;
            if (view.Audience != null)
            {
                if ((view.Audience.Groups != null && view.Audience.Groups.Count > 0) || (view.Audience.Users != null && view.Audience.Users.Count > 0))
                    serialziedAudience = Common.Serializer.Serialize(view.Audience, true);
            }
            string serializedSettings = view.Settings != null ? Common.Serializer.Serialize(view.Settings) : null;
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_Insert", view.ViewId, view.Name, view.Title, view.Url, view.ModuleId,
               serialziedAudience, serialziedContent, serializedSettings, view.Type);
            return (recordesEffected > 0);
        }
        public bool UpdateView(View view)
        {
            string serialziedContent = null;
            if (view.ViewContent != null)
            {
                if ((view.ViewContent.BodyContents != null && view.ViewContent.BodyContents.Count > 0) || (view.ViewContent.SummaryContents != null && view.ViewContent.SummaryContents.Count > 0))
                    serialziedContent = Common.Serializer.Serialize(view.ViewContent, true);
            }
            string serialziedAudience = null;
            if (view.Audience != null)
            {
                if ((view.Audience.Groups != null && view.Audience.Groups.Count > 0) || (view.Audience.Users != null && view.Audience.Users.Count > 0))
                    serialziedAudience = Common.Serializer.Serialize(view.Audience, true);
            }
            string serializedSettings = view.Settings != null ? Common.Serializer.Serialize(view.Settings) : null;
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_Update", view.ViewId, view.Name,view.Title, view.Url, view.ModuleId,
               serialziedAudience, serialziedContent, serializedSettings, (int)view.Type);
            return (recordesEffected > 0);
        }
        public bool DeleteView(Guid viewId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_Delete", viewId);
            return (recordesEffected > 0);
        }

        public bool UpdateViewRank(Guid viewId, Guid moduleId, int rank)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_UpdateRank", viewId,moduleId, rank);
            return (recordesEffected > 0);
        }
        public bool AreViewsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[sec].[View]", ref updateHandle);
        }

        #endregion

        #region Mappers
        private View ViewMapper(IDataReader reader)
        {

            View instance = new View
            {
                ViewId = GetReaderValue<Guid>(reader,"Id"),
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Url = reader["Url"] as string,
                ModuleId = GetReaderValue <Guid>(reader,"Module"),
                ActionNames = GetReaderValue<string>(reader, "ActionNames"),
                Audience = ((reader["Audience"] as string) != null) ? Common.Serializer.Deserialize<AudienceWrapper>(reader["Audience"] as string) : null,
                ViewContent = ((reader["Content"] as string) != null) ? Common.Serializer.Deserialize<ViewContent>(reader["Content"] as string) : null,
                Settings = reader["Settings"] != DBNull.Value ? Common.Serializer.Deserialize(reader["Settings"] as string) as ViewSettings : null,
                Rank = GetReaderValue<int>(reader, "Rank"),
                Type = (int)reader["Type"],

            };


            return instance;
        }
        #endregion
    }
}
