using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntityRuntimeEditor
    {
        public GenericBEDefinitionSettings GenericBEDefinitionSettings { get; set; }
        public GenericBusinessEntity GenericBusinessEntity { get; set; }
        public string DefinitionTitle { get; set; }
        public string TitleFieldName { get; set; }
    }
}
