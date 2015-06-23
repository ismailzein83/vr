using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class DynamicPagesManagementController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<DynamicPage> GetDynamicPages()
        {
            DynamicPagesManager manager = new DynamicPagesManager();
            return manager.GetDynamicPages();
        }
        [HttpGet]
        public List<Widget> GetWidgets()
        {
            DynamicPagesManager manager = new DynamicPagesManager();
            return manager.GetWidgets();
        }

    }
}