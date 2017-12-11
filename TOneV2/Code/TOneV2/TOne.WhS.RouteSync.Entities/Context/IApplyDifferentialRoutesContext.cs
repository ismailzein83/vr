using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public interface IApplyDifferentialRoutesContext
    {
        string SwitchId { get; }

        string SwitchName { get; }

        List<ConvertedRoute> ConvertedUpdatedRoutes { get; }

        Action<Exception, bool> WriteBusinessHandledException { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }
    }

    public class ApplyDifferentialRoutesContext : IApplyDifferentialRoutesContext
    {
        public string SwitchId { get; set; }

        public string SwitchName { get; set; }

        public List<ConvertedRoute> ConvertedUpdatedRoutes { get; set; }

        public Action<Exception, bool> WriteBusinessHandledException { get; set; }

        public SwitchSyncOutput SwitchSyncOutput { get; set; }
    }
}