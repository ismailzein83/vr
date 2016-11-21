using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class CloseCodesOutput
    {
        public string Message { get; set; }
        public IEnumerable<CodeItem> ClosedCodes { get; set; }

        public ValidationOutput Result { get; set; }
    }
}
