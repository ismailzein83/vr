using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class NewCodeOutput
    {
        public string Message { get; set; }
        public CodeItem CodeItem { get; set; }
        public NewCodeOutputResult Result { get; set; }
    }
}
