using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Entities
{
    public class CodeNode : CodeTree
    {
        public CodeNode(char codeDigit, List<string> distinctCodes, int comparePosition) : base (distinctCodes, comparePosition)
        {
            this.CodeDigit = codeDigit;
            
        }

        public Char CodeDigit { get; private set; }
    }
}
