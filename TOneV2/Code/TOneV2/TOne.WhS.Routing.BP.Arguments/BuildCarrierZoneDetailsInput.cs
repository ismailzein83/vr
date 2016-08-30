using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class BuildCarrierZoneDetailsInput : BaseProcessInputArgument
    {
        public int MinCarrierId { get; set; }

        public int MaxCarrierId { get; set; }

        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle# By Ids: From {0} to {1}.", MinCarrierId, MaxCarrierId);
        }
    }
}
