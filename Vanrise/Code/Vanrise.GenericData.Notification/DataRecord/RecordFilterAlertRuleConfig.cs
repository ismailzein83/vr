using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class RecordAlertRuleSettings : DataRecordAlertRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("AAF0C72D-1C8F-47DA-AC11-DD7819B93351"); } }

        public List<RecordAlertRuleConfig> RecordAlertRuleConfigs { get; set; }

        public override bool IsRuleMatched(IDataRecordAlertRuleSettingsIsMatchedContext context)
        {
            if (RecordAlertRuleConfigs == null || RecordAlertRuleConfigs.Count == 0)
                return false;

            DataRecordDictFilterGenericFieldMatchContext filterContext = new DataRecordDictFilterGenericFieldMatchContext(context.OutputRecords, context.DataRecordTypeId);
            RecordFilterManager manager = new RecordFilterManager();
            VRAlertLevelManager alertLevelManager = new VRAlertLevelManager();

            IOrderedEnumerable<RecordAlertRuleConfig> orderedRecordAlertRuleConfig = RecordAlertRuleConfigs.OrderByDescending(itm => alertLevelManager.GetAlertLevelWeight(itm.AlertLevelId));
            foreach (RecordAlertRuleConfig recordAlertRuleConfig in orderedRecordAlertRuleConfig)
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