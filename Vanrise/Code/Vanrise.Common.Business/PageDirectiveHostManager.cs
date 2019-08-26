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
    public class PageDirectiveHostManager
    {
        #region Public Methods

        public bool DoesUserHaveViewAccess(PageDirectiveHost pageHost)
        {
            return DoesUserHaveAccess(pageHost.PermissionName);
        }

        public PageDirectiveHostInfo GetPageDirectiveHostInfo(Guid viewId)
        {

            View view = BEManagerFactory.GetManager<IViewManager>().GetView(viewId);
            PageDirectiveHostViewSettings settings = view.Settings as PageDirectiveHostViewSettings;
            settings.PageDirectiveHost.ThrowIfNull("Page Directive Host");
            return PageDirectiveHostInfoMapper(settings.PageDirectiveHost);

        }

        #endregion

        #region Private Methods

        private bool DoesUserHaveAccess(string permissionName)
        {
            return ContextFactory.GetContext().IsAllowed(permissionName);
        }

        PageDirectiveHostInfo PageDirectiveHostInfoMapper(PageDirectiveHost PageDirectiveHost)
        {
            return new PageDirectiveHostInfo()
            {
                Title = PageDirectiveHost.Title,
                Directive = PageDirectiveHost.Directive,
                Data = PageDirectiveHost.Data
            };
        }
        #endregion
    }

}
