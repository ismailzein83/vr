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
        public abstract Guid ConfigId { get; }

        public virtual bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            return true;
        }
    }
}
