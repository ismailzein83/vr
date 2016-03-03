using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEDefinitionSettings:BusinessEntityDefinitionSettings
    {
        public int DataRecordTypeId { get; set; }
        public GenericEditor EditorDesign { get; set; }
        public GenericManagement ManagementDesign { get; set; }
        
    }
}
