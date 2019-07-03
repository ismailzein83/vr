using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GetGenericEditorColumnsInfoInput
    {
        public ListRecordRuntimeViewType ListRecordViewType { get; set; }
        public Guid DataRecordTypeId { get; set; }
    }
}
