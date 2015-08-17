using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class GroupSummary<T>
    {
        public KeyColumn[] GroupKeyValues { get; set; }

        public T Data { get; set; }


        
    }
}