using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class CustomCodeCompilationOutput
    {
        public List<string> ErrorMessages { get; set; }

        public bool CompilationSucceeded { get; set; }
    }
}
