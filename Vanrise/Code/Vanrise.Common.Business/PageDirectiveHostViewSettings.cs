using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class PageDirectiveHostViewSettings : ViewSettings
    {

        public PageDirectiveHost PageDirectiveHost { get; set; }
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/Common/Views/PageDirectiveHost/PageDirectiveHostManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }

        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            return new PageDirectiveHostManager().DoesUserHaveViewAccess(this.PageDirectiveHost);
        }
    }

    public class PageDirectiveHost
    {
        public string PermissionName { set; get; }
        public string Directive { get; set; }
        public string Title { get; set; }
        public PageHostData Data { get; set; }
    }
     
    public class PageDirectiveHostInfo
    {
        public string Directive { get; set; }

        public string Title { get; set; }
        public PageHostData Data { get; set; }

    }

    public abstract class PageHostData{ };
}
