using System.Web;
using System.Web.Mvc;

namespace SOM.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            Vanrise.Web.FilterConfig.RegisterGlobalFilters(filters);
        }
    }
}
