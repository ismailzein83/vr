using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class CodePrefix : ICode
    {
        public string Code { get; set; }

        public bool IsCodeDivided { get; set; }

        public int CodeCount { get; set; }
    }
}
