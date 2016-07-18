﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountDetail
    {
        public Account Entity { get; set; }
        public string AccountTypeTitle { get; set; }
        public int DirectSubAccountCount { get; set; }
        public int TotalSubAccountCount { get; set; }
        public bool CanAddSubAccounts { get; set; }
        public IEnumerable<ActionDefinitionInfo>  ActionDefinitions { get; set; }
        public string  StatusDesciption { get; set; }
        public string StatusColor  { get; set; }
    }
}
