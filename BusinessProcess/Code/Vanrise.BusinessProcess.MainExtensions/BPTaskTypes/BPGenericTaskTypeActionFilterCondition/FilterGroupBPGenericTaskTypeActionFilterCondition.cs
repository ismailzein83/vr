using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess.Business;
using Vanrise.GenericData.Business;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskTypes
{
    public class FilterGroupBPGenericTaskTypeActionFilterCondition : BPGenericTaskTypeActionFilterCondition
    {
        public override Guid ConfigId { get { return new Guid("68DEA8B2-8817-40EE-977D-4EE683CE5092"); } }
        public RecordFilter FilterGroup { get; set; }
        public override bool IsFilterMatch(IBPGenericTaskTypeActionFilterConditionContext context)
        {
            var genericTaskData = context.Task.TaskData.CastWithValidate<BPGenericTaskData>("genericTaskData");
            if (genericTaskData.FieldValues == null)
                genericTaskData.FieldValues = new Dictionary<string, dynamic>();
            var filterGroup = FilterGroup.CastWithValidate<RecordFilterGroup>("filterGroup");
            BPTaskTypeManager bpTaskTypeManager = new BPTaskTypeManager();
            var taskType = bpTaskTypeManager.GetBPTaskType(context.Task.TypeId);
            taskType.ThrowIfNull("taskType", context.Task.TypeId);
            taskType.Settings.ThrowIfNull("taskType.Settings", context.Task.TypeId);
            var genericTaskType = taskType.Settings.CastWithValidate<BPGenericTaskTypeSettings>("genericTaskType");
            RecordFilterManager recordFilterManager = new RecordFilterManager();
            if (!recordFilterManager.IsFilterGroupMatch(filterGroup, new DataRecordDictFilterGenericFieldMatchContext(genericTaskData.FieldValues, genericTaskType.RecordTypeId)))
                return false;
            return true;
        }
    }
}
