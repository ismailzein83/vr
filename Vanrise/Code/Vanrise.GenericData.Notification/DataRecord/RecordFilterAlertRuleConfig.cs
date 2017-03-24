using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class RecordAlertRuleSettings : DataRecordAlertRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("AAF0C72D-1C8F-47DA-AC11-DD7819B93351"); } }

        public List<RecordAlertRuleConfig> RecordAlertRuleConfigs { get; set; }

        public override bool IsRuleMatched(IDAProfCalcAlertRuleIsMatchedContext context)
        {
            if (RecordAlertRuleConfigs == null || RecordAlertRuleConfigs.Count == 0)
                return false;

            DataRecordDictFilterGenericFieldMatchContext filterContext = new DataRecordDictFilterGenericFieldMatchContext(context.OutputRecords, context.DataRecordTypeId);
            RecordFilterManager manager = new RecordFilterManager();

            //TODO: RecordAlertRuleConfigs should be ordered by alert level id
            foreach (RecordAlertRuleConfig recordAlertRuleConfig in RecordAlertRuleConfigs)
            {
                if (manager.IsFilterGroupMatch(recordAlertRuleConfig.FilterGroup, filterContext))
                {
                    context.AlertLevelId = recordAlertRuleConfig.AlertLevelId;
                    context.Actions = recordAlertRuleConfig.Actions;
                    return true;
                }
            }

            return false;
        }
    }

    public class RecordAlertRuleConfig
    {
        public Guid AlertLevelId { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public List<VRAction> Actions { get; set; }
    }
}