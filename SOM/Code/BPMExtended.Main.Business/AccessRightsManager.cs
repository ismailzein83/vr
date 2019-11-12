using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class AccessRightsManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        public bool CanManageResidential()
        {
            return BPM_UserConnection.DBSecurityEngine.GetCanExecuteOperation("CanManageResidential");
        }
        public bool CanManageEntreprise()
        {
            return BPM_UserConnection.DBSecurityEngine.GetCanExecuteOperation("CanManageEntreprise");
        }
        public bool CanManageOfficial()
        {
            return BPM_UserConnection.DBSecurityEngine.GetCanExecuteOperation("CanManageOfficial");
        }
    }
}
