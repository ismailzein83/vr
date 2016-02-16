using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class OperatorAccountQuery
    {
        public List<int> OperatorAccountsIds { get; set; }
        public List<int> OperatorProfilesIds { get; set; }
        public string Name { get; set; }
    }
}
