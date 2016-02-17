using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BusinessEntityMatchContext : IBusinessEntityMatchContext
    {
        public object FieldValue { get; set; }

        public object FilterValue { get; set; }
    }
}
