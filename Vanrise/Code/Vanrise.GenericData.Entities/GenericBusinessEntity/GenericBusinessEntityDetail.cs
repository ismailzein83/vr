using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntityDetail
    {
        public GenericBusinessEntityValues FieldValues { get; set; }
    }
    public class GenericBusinessEntityValues : Dictionary<string, GenericBusinessEntityValue>
    {

    }
    public class GenericBusinessEntityValue
    {
        public object Value { get; set; }
        public string Description { get; set; }
    }

}
