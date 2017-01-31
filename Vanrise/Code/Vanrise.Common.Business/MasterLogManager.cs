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
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class MasterLogManager
    {
        #region Public Methods

        public bool DoesUserHaveViewAccess(List<LogViewItem> items)
        {
            foreach (var item in items)
            {
                if(DoesUserHaveAccess(item.PermissionName))
                    return true;
            }
            return false;
        }

        public List<LogViewInfo> GetMasterLogDirectives(Guid viewId)
        {
            List<LogViewInfo> directives = new List<LogViewInfo>();
            View view = BEManagerFactory.GetManager<IViewManager>().GetView(viewId);
            MasterLogViewSettings settings = view.Settings as MasterLogViewSettings;
            foreach (var i in settings.Items)
                if (DoesUserHaveAccess(i.PermissionName))
                    directives.Add(new LogViewInfo() { 
                         Directive=i.Directive,
                         Title = i.Title
                    });
            return directives;

        }

        #endregion

        #region Private Methods

        private bool DoesUserHaveAccess(string permissionName)
        {

            return ContextFactory.GetContext().IsAllowed(permissionName);

        }
        #endregion
    }

}
