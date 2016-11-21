using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class MoveCodeOutput
    {
        public string Message { get; set; }

        public IEnumerable<CodeItem> NewCodes { get; set; }

        public ValidationOutput Result { get; set; }
    }
}
