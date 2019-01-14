﻿using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class ExcludedCodes : RoutingExcludedDestinations
    {
        public override Guid ConfigId { get { return new Guid("BA55C956-D395-4776-88E0-CFA0B8565F50"); } }

        public List<string> Codes { get; set; }

        public override bool IsExcludedDestination(IRoutingExcludedDestinationContext context)
        {
            context.ThrowIfNull("context");

            if (string.IsNullOrEmpty(context.Code))
                return false;

            if (this.Codes == null)
                throw new NullReferenceException(string.Format("Excluded Codes not found for Rule {0}", context.RuleId));

            if (this.Codes.Contains(context.Code))
                return true;

            return false;
        }

        public override RoutingExcludedDestinationData GetRoutingExcludedDestinationData()
        {
            return new RoutingExcludedDestinationData() { ExcludedCodes = this.Codes };
        }

        public override string GetDescription()
        {
            if (this.Codes == null)
                return string.Empty;

            return string.Join<string>(", ", this.Codes);
        }
    }
}