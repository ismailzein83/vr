using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class MasterLogViewSettings : ViewSettings
    {

        public List<LogViewItem> Items { get; set; }
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/Common/Views/MasterLog/MasterLogManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }

        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            return new MasterLogManager().DoesUserHaveViewAccess(this.Items);
        }
    }

    public class LogViewItem
    {
        public string PermissionName { set; get; }

        public string Directive { get; set; }

        public string Title { get; set; }

    }

    public class LogViewInfo
    {
        public string Directive { get; set; }

        public string Title { get; set; }

    }
}
