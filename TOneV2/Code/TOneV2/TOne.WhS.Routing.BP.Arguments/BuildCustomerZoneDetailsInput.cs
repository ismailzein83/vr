using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class BuildCustomerZoneDetailsInput : BaseProcessInputArgument
    {
        public List<RoutingCustomerInfo> CustomerInfos { get; set; }

        public int RoutingDatabaseId { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public int MinCustomerId { get; set; }

        public int MaxCustomerId { get; set; }

        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle# By Ids: From {0} to {1}.", MinCustomerId, MaxCustomerId);
        }
    }
}
