﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class SourceAccountData : ITargetBE
    {
        public Account Account { get; set; }
        public object SourceBEId
        {
            get { throw new NotImplementedException(); }
        }

        public object TargetBEId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
