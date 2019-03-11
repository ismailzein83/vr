using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskType
{
    public class BPGenericTaskTypeSettings : BaseBPTaskTypeSettings
    {
        public override Guid ConfigId => new Guid("0675F4DE-CB92-4F57-ADF2-00F5BA72E5F5");

        public override string Editor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {

            }
        }

        public Guid RecordTypeId { get; set; }

        public GenericData.Entities.VRGenericEditorDefinitionSetting EditorSettings { get; set; }
    }

    public class BPGenericTaskData : BPTaskData
    {
        public Dictionary<string, dynamic> FieldValues { get; set; }
    }

    public class BPGenericTaskExecutionInformation : BPTaskExecutionInformation
    {

    }
}
