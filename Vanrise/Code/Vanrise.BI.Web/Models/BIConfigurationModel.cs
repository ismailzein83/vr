using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vanrise.BI.Web.Models
{
    public class BIMeasureModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string RequiredPermissions { get; set; }
    }
    public class BIEntityModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}