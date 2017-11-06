﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class RealTimeReportSettings : AnalyticReportSettings
    {
        public override Guid ConfigId { get { return new Guid("635B2CE2-F787-4F46-832E-69B78D422FD5"); } }

        public List<Guid> AnalyticTableIds { get; set; }

        public RealTimeReportSearchSettings SearchSettings { get; set; }

        public List<RealTimeReportWidget> Widgets { get; set; }

        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {
            var analyticTable = BEManagerFactory.GetManager<IAnalyticTableManager>();
            var analyticItem = BEManagerFactory.GetManager<IAnalyticItemConfigManager>();

            foreach (Guid id in this.AnalyticTableIds)
            {
                if (analyticTable.DoesUserHaveAccess(context.UserId, id) == false)
                    return false;
            }
            foreach (var w in this.Widgets)
            {
                if (analyticItem.DoesUserHaveAccess(context.UserId, w.AnalyticTableId, w.GetMeasureNames()) == false)
                    return false;
            }
            return true;
        }
    }
}
