using System;
using System.Collections.Generic;

namespace Mediation.Mobilis.Business
{
    public class MobilisDataSourceManager
    {
        public static Guid s_SwitchTrunkIdentificationRule = new Guid("FFBF9592-57E0-497E-9D16-4B38D27A8A87");

        public const int MSOriginatingType = 0;
        public const int MSTerminatingType = 1;
        public const int TransitType = 5;
        public const int CallForwardingType = 100;

        public bool IsMobilisR13MultiLegCDR(dynamic cdr)
        {
            var mappingRuleManager = new Vanrise.GenericData.Transformation.MappingRuleManager();

            var inTrunkSwitchTarget = new Vanrise.GenericData.Entities.GenericRuleTarget() { TargetFieldValues = new Dictionary<string, object>() };
            inTrunkSwitchTarget.TargetFieldValues.Add("Direction", 1);
            inTrunkSwitchTarget.TargetFieldValues.Add("Trunks", cdr.IncomingRoute);
            var inTrunkSwitchRule = mappingRuleManager.GetMatchRule(s_SwitchTrunkIdentificationRule, inTrunkSwitchTarget);

            var outTrunkSwitchTarget = new Vanrise.GenericData.Entities.GenericRuleTarget() { TargetFieldValues = new Dictionary<string, object>() };
            outTrunkSwitchTarget.TargetFieldValues.Add("Direction", 2);
            outTrunkSwitchTarget.TargetFieldValues.Add("Trunks", cdr.OutgoingRoute);
            var outTrunkSwitchRule = mappingRuleManager.GetMatchRule(s_SwitchTrunkIdentificationRule, outTrunkSwitchTarget);

            cdr.IsInTrunkSwitch = false;
            if (inTrunkSwitchRule != null && (bool)inTrunkSwitchRule.Settings.Value)
                cdr.IsInTrunkSwitch = true;

            cdr.IsOutTrunkSwitch = false;
            if (outTrunkSwitchRule != null && (bool)outTrunkSwitchRule.Settings.Value)
                cdr.IsOutTrunkSwitch = true;

            if (cdr.RecordType == MSOriginatingType && cdr.IsOutTrunkSwitch)
                return true;

            if (cdr.RecordType == MSTerminatingType && cdr.IsInTrunkSwitch)
                return true;

            if (cdr.RecordType == CallForwardingType && (cdr.IsInTrunkSwitch || cdr.IsOutTrunkSwitch))
                return true;

            if (cdr.RecordType == TransitType && (cdr.IsInTrunkSwitch == !cdr.IsOutTrunkSwitch))
                return true;

            return false;
        }
    }
}