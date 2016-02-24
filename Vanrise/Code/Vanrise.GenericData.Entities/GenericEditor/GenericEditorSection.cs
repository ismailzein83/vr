using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericEditorSection
    {
        public string SectionTitle { get; set; }
        public List<GenericEditorRow> Rows { get; set; }
    }
}
