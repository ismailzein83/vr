using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace QM.CLITester.Entities.VRObjectTypes
{
    public enum ScheduleTestCallField
    {
        TaskName = 0
    }

    public class ScheduleTestCallPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("ae69adba-fa08-48c7-896c-961c75a0a011"); } }

        public ScheduleTestCallField ScheduleTestCallField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            ScheduleTestCallInfo scheduleTestCallInfo = context.Object as ScheduleTestCallInfo;

            if (scheduleTestCallInfo == null)
                throw new NullReferenceException("scheduleTestCallInfo");

            switch (this.ScheduleTestCallField)
            {
                case ScheduleTestCallField.TaskName: return scheduleTestCallInfo.TaskName;
            }

            return null;
        }
    }
}
