﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticHistoryReportSettings : AnalyticReportSettings
    {
        public override Guid ConfigId { get { return new Guid("E5FB0790-5428-44B4-BB1F-4F79B69CD6EF"); } }

        public List<Guid> AnalyticTableIds { get; set; }

        public AnalyticHistoryReportSearchSettings SearchSettings { get; set; }

        public List<AnalyticHistoryReportWidget> Widgets { get; set; }

        public override bool DoesUserHaveAccess(Security.Entities.IViewUserAccessContext context)
        {
            var analyticTable = BEManagerFactory.GetManager<IAnalyticTableManager>();
            var analyticItem = BEManagerFactory.GetManager<IAnalyticItemConfigManager>();

            foreach (Guid id in this.Widgets.Select(itm => itm.AnalyticTableId).Distinct())
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
