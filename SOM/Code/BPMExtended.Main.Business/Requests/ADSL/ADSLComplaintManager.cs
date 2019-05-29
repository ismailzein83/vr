using System;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class ADSLComplaintManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion

        #region public
        public void PostADSLComplaintToOM(Guid requestId)
        {
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var update = new Update(connection, "StRequestHeader").Set("StStatusId", Column.Parameter("8057E9A4-24DE-484D-B202-0D189F5B7758"))
                .Where("StRequestId").IsEqual(Column.Parameter(requestId));
            update.Execute();
        }

        #endregion
    }
}
