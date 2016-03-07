using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntityDetail
    {
        public GenericBusinessEntity Entity { get; set; }

        public List<string> FieldValueDescriptions { get; set; }
    }
}
