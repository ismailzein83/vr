using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DimensionValue : ISetRecordDescription
    {
        public Object Value { get; set; }
        public string Name { get; set; }

        public void SetDescription(ISetRecordDescriptionContext context)
        {
            Name = context.Description;
        }
    }
}
