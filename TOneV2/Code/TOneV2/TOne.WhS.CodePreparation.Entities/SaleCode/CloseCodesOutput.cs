using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class CloseCodesOutput
    {
        public string Message { get; set; }
        public IEnumerable<CodeItem> NewCodes { get; set; }

        public ValidationOutput Result { get; set; }
    }
}
