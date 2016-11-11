using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities 
{
    public class CodecProfileEditorRuntime
    {
        public CodecProfile Entity { get; set; }
        public List<CodecDef> CodecDefList { get; set; }
    }
}
