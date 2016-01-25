﻿using System;
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

        public bool AddView(View view, out int insertedId)
        {
            string serialziedContent = null;
            if (view.ViewContent.BodyContents.Count > 0 || view.ViewContent.SummaryContents.Count > 0)
                serialziedContent = Common.Serializer.Serialize(view.ViewContent, true);
            string serialziedAudience = null;
            if ((view.Audience.Groups != null && view.Audience.Groups.Count > 0) || (view.Audience.Users != null && view.Audience.Users.Count > 0))
                serialziedAudience = Common.Serializer.Serialize(view.Audience, true);
            object viewId;
            string url = "#/viewwithparams/Security/Views/DynamicPages/DynamicPagePreview";
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_Insert", out viewId, view.Name, view.Name, url, view.ModuleId, null,
               serialziedAudience, serialziedContent, ViewType.Dynamic);
            insertedId = (recordesEffected > 0) ? (int)viewId : -1;
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

        public bool UpdateViewRank(int viewId, int rank)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_UpdateRank", viewId, rank);
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
                ViewId = (int)reader["Id"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Url = reader["Url"] as string,
                ModuleId = (int)reader["Module"],
                RequiredPermissions = GetReaderValue<string>(reader, "RequiredPermissions"),
                Audience = ((reader["Audience"] as string) != null) ? Common.Serializer.Deserialize<AudienceWrapper>(reader["Audience"] as string) : null,
                ViewContent = ((reader["Content"] as string) != null) ? Common.Serializer.Deserialize<ViewContent>(reader["Content"] as string) : null,
                Rank = GetReaderValue<int>(reader, "Rank"),
                Type = (ViewType)reader["Type"],

            };


            return instance;
        }
        #endregion
      
    
      
    }


}
