using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEField360DegreeViewData
    {
        public Guid BusinessEntityDefinitionId { get; set; }

        public string IdFieldName { get; set; }

        public BusinessEntityDefinitionSettings GenericBEDefinitionSettings { get; set; }

    }
}
