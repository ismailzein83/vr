using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericEditor
    {
        public int DataRecordTypeId { get; set; }

        public List<GenericEditorSection> Sections { get; set; }
    }
}
