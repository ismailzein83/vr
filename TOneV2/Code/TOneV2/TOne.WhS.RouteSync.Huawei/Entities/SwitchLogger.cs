using System;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public abstract class SwitchLogger
    {
        public abstract Guid ConfigId { get; }

        public bool IsActive { get; set; }

        public abstract void LogRouteCases(ILogRouteCasesContext context);

        public abstract void LogRoutes(ILogRoutesContext context);

        public abstract void LogCommands(ILogCommandsContext context);
    }
}