using System;
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
