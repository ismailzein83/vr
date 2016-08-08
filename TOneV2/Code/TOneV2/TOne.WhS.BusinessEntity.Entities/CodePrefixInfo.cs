using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CodePrefixInfo
    {
        public string CodePrefix { get; set; }

        public int Count { get; set; }

        public override int GetHashCode()
        {
            return CodePrefix.GetHashCode();
        }
    }
}
