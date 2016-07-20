using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalanceDetail
    {
        public LiveBalance Entity { get; set; }

        public AccountInfo AccountInfo { get; set; }
    }
}
