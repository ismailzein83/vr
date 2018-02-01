using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BEDefinitionSettingsTryGetRecordTypeFieldsContext : IBEDefinitionSettingsTryGetRecordTypeFieldsContext
    {
        public BusinessEntityDefinition BEDefinition { get; set; }
    }
}
