using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class CustomCodeTaskCompilationOutput
    {
        public List<string> ErrorMessages { get; set; }
        public bool Result { get; set; }
    }
}
