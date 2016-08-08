using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticReportSettings
    {
        public int ConfigId { get; set; }

        public virtual bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            return true;
        }
    }
}
