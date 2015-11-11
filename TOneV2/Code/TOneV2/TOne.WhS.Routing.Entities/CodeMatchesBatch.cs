using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CodeMatchesBatch
    {
        public CodeMatchesBatch()
        {
            this.CodeMatches = new List<CodeMatches>();
        }

        public List<CodeMatches> CodeMatches { get; set; }
    }
}
