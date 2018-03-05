using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using System.Linq;

namespace TOne.WhS.Routing.Business
{
    public class ExcludedRanges : RoutingExcludedDestinations
    {
        public override Guid ConfigId { get { return new Guid("BBBD7994-68C5-423D-8A51-3B29D3C3A43A"); } }

        public List<CodeRange> CodeRanges { get; set; }

        public override bool IsExcludedDestination(IRoutingExcludedDestinationContext context)
        {
            context.ThrowIfNull("context");

            if (string.IsNullOrEmpty(context.Code))
                throw new NullReferenceException("context.Code");

            if (this.CodeRanges == null)
                throw new NullReferenceException(string.Format("Excluded Code Ranges not found for Rule {0}", context.RuleId));

            foreach (var codeRange in CodeRanges)
            {
                if (codeRange == null)
                    throw new NullReferenceException(string.Format("Invalid Excluded Code Range data for Rule {0}", context.RuleId));

                if (codeRange.FromCode.Length != codeRange.ToCode.Length)
                    throw new Vanrise.Entities.VRBusinessException(string.Format("Range bounds (From: '{0}', To: '{1}') have different length.", codeRange.FromCode, codeRange.ToCode));

                if (codeRange.FromCode.Length != context.Code.Length) //codeRange.FromCode.Length == codeRange.ToCode.Length
                    continue;

                if (string.Compare(context.Code, codeRange.FromCode) >= 0 && string.Compare(codeRange.ToCode, context.Code) >= 0)
                    return true;
            }

            return false;
        }

        public override RoutingExcludedDestinationData GetRoutingExcludedDestinationData()
        {
            return new RoutingExcludedDestinationData() { CodeRanges = this.CodeRanges };
        }

        public override string GetDescription()
        {
            if (this.CodeRanges == null)
                return string.Empty;

            return string.Join<string>(", ", this.CodeRanges.Select(itm => itm.GetDescription()));
        }
    }
}