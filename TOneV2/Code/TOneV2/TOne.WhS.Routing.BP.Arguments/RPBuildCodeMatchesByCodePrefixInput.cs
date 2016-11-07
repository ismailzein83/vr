﻿using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RPBuildCodeMatchesByCodePrefixInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Dictionary<string, int> SupplierCodeServiceRuntimeProcessIds { get; set; }

        public int RoutingDatabaseId { get; set; }

		public bool ShouldApplyCodeZoneMatch { get; set; }

        public IEnumerable<CodePrefix> CodePrefixGroup { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public string CodePrefixGroupDescription { get; set; }

        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle#: {0}", CodePrefixGroupDescription);
        }
    }
}
