using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.Business
{
    public class HuaweiFTPLogger : SwitchLogger
    {
        public override Guid ConfigId { get { return new Guid("6F5E6051-DDAB-411C-88F5-C8927417FD3C"); } }

        public FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }

        public override void LogRouteCases(ILogRouteCasesContext context)
        {
            throw new NotImplementedException();
        }

        public override void LogCarrierMappings(ILogCarrierMappingsContext context)
        {
            throw new NotImplementedException();
        }

        public override void LogRoutes(ILogRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public override void LogCommands(ILogCommandsContext context)
        {
            throw new NotImplementedException();
        }

    }
}