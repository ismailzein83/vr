using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
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
               serialziedAudience, serialziedContent, serializedSettings,view.Type);
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
        
        public void GenerateScript(List<View> views, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var view in views)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10})",
                    view.ViewId, view.Name, view.Title, GetStringSQLValue(view.Url), GetStringSQLValue(view.ModuleId.HasValue ? view.ModuleId.Value.ToString() : null), GetStringSQLValue(view.ActionNames),
                    GetStringSQLValue(view.Audience != null ? Serializer.Serialize(view.Audience) : null), GetStringSQLValue(view.ViewContent != null ? Serializer.Serialize(view.ViewContent) : null),
                    GetStringSQLValue(view.Settings != null ? Serializer.Serialize(view.Settings) : null), GetStringSQLValue(view.Type.ToString()), view.Rank);
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);", scriptBuilder);
            addEntityScript("[sec].[View]", script);
        }

        private string GetStringSQLValue(string value)
        {
            return value != null ? string.Format("'{0}'", value) : "null";
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
                Type = GetReaderValue<Guid>(reader,"Type"),

            };


            return instance;
        }
        #endregion
    }
}
