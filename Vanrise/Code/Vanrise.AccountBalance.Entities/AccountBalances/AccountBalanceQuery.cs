﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceQuery
    {
        public List<long> AccountsIds { get; set; }
        public Guid AccountTypeId { get; set; }
        public int Top { get; set; }

    }
}
