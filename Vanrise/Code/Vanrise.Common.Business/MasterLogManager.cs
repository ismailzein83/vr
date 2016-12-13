using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using System.Globalization;
using Vanrise.Common.Business;
using System.ComponentModel;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class MasterLogManager
    {
        SecurityManager _secManager;
        #region Public Methods

        public MasterLogManager()
        {
            _secManager = new SecurityManager();
        }
        public bool DoesUserHaveViewAccess(int userId, List<LogViewItem> items)
        {
            foreach (var item in items)
            {
                if(DoesUserHaveAccess(userId, item.PermissionName))
                    return true;
            }
            return false;
        }

        public List<LogViewInfo> GetMasterLogDirectives(Guid viewId)
        {
            List<LogViewInfo> directives = new List<LogViewInfo>();
            View view = new ViewManager().GetView(viewId);
            MasterLogViewSettings settings = view.Settings as MasterLogViewSettings;
            int userId =  SecurityContext.Current.GetLoggedInUserId();
            foreach (var i in settings.Items)
                if (DoesUserHaveAccess(userId, i.PermissionName))
                    directives.Add(new LogViewInfo() { 
                         Directive=i.Directive,
                         Title = i.Title
                    });
            return directives;

        }

        #endregion

        #region Private Methods

        private bool DoesUserHaveAccess(int userId, string permissionName)
        {

            return _secManager.IsAllowed(permissionName, userId);

        }
        #endregion
    }

}
