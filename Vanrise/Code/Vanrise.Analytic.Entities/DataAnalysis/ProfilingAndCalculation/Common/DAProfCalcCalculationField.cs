using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcCalculationField
    {
        public string FieldName { get; set; }

        public DataRecordFieldType FieldType { get; set; }

        public string Expression { get; set; }
    }
}
