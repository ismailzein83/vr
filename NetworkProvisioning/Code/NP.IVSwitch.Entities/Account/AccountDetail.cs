using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class AccountDetail
    {
        public Account Entity { get; set; }
        public string TypeDescription { get; set; }
        public string CurrentStateDescription { get; set; }
    }
}
