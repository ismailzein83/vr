using System;
using System.Collections.Generic;
using Mediation.Generic.Entities;

namespace Mediation.Mobilis.Business
{
    public class MobilisDataSourceManager
    {
        public static Guid s_SwitchTrunkIdentificationRule = new Guid("FFBF9592-57E0-497E-9D16-4B38D27A8A87");

        public const int InDirection = 1;
        public const int OutDirection = 2;

        public const int MSOriginatingType = 0;
        public const int MSTerminatingType = 1;
        public const int TransitType = 5;
        public const int CallForwardingType = 100;

        public CDRState GetMobilisR13CDRType(dynamic cdr)
        {
            var mappingRuleManager = new Vanrise.GenericData.Transformation.MappingRuleManager();

            cdr.IsInTrunkSwitch = false;
            string incomingRoute = cdr.IncomingRoute;
            if (!string.IsNullOrEmpty(incomingRoute))
            {
                var inTrunkSwitchTarget = new Vanrise.GenericData.Entities.GenericRuleTarget();
                inTrunkSwitchTarget.TargetFieldValues = new Dictionary<string, object>();
                inTrunkSwitchTarget.TargetFieldValues.Add("Direction", InDirection);
                inTrunkSwitchTarget.TargetFieldValues.Add("Trunks", incomingRoute);
                var inTrunkSwitchRule = mappingRuleManager.GetMatchRule(s_SwitchTrunkIdentificationRule, inTrunkSwitchTarget);

                if (inTrunkSwitchRule != null && (bool)inTrunkSwitchRule.Settings.Value)
                    cdr.IsInTrunkSwitch = true;
            }
            else
            {
                cdr.IsInTrunkSwitch = true;
            }

            cdr.IsOutTrunkSwitch = false;
            string outgoingRoute = cdr.OutgoingRoute;
            if (!string.IsNullOrEmpty(outgoingRoute))
            {
                var outTrunkSwitchTarget = new Vanrise.GenericData.Entities.GenericRuleTarget();
                outTrunkSwitchTarget.TargetFieldValues = new Dictionary<string, object>();
                outTrunkSwitchTarget.TargetFieldValues.Add("Direction", OutDirection);
                outTrunkSwitchTarget.TargetFieldValues.Add("Trunks", outgoingRoute);
                var outTrunkSwitchRule = mappingRuleManager.GetMatchRule(s_SwitchTrunkIdentificationRule, outTrunkSwitchTarget);

                if (outTrunkSwitchRule != null && (bool)outTrunkSwitchRule.Settings.Value)
                    cdr.IsOutTrunkSwitch = true;
            }
            else
            {
                cdr.IsOutTrunkSwitch = true;
            }

            if (cdr.RecordType == TransitType && cdr.IsInTrunkSwitch && cdr.IsOutTrunkSwitch)
                return CDRState.Ignore;

            if (cdr.RecordType == TransitType && cdr.IsInTrunkSwitch == !cdr.IsOutTrunkSwitch)
                return CDRState.MultiLeg;

            if (cdr.RecordType == MSOriginatingType && cdr.IsOutTrunkSwitch)
                return CDRState.MultiLeg;

            if (cdr.RecordType == MSTerminatingType && cdr.IsInTrunkSwitch)
                return CDRState.MultiLeg;

            if (cdr.RecordType == CallForwardingType && (cdr.IsInTrunkSwitch || cdr.IsOutTrunkSwitch))
                return CDRState.MultiLeg;

            return CDRState.Normal;
        }
    }
}