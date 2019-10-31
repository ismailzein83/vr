using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcGroupingField
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public bool IsRequired { get; set; }

        public bool IsSelected { get; set; }

        public DataRecordFieldType FieldType { get; set; }
    }
}
