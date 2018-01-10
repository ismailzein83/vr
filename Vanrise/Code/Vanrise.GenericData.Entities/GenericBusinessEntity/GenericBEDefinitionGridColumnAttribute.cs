using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEDefinitionGridColumnAttribute
    {
        public GridColumnAttribute Attribute { get; set; }
        public string Name { get; set; }
    }
}
