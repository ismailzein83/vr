using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericFilterField
    {
        public string FieldTitle { get; set; }
        public bool IsRequired { get; set; }
        public string FieldPath { get; set; }
    }
}
