using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CodeRange
    {
        public string FromCode { get; set; }

        public string ToCode { get; set; }

        public string GetDescription()
        {
            return string.Concat(FromCode, " -> ", ToCode);
        }
    }
}