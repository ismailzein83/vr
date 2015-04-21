﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.LCR.Entities
{
    public abstract class BaseCarrierAccountSet
    {
        public abstract string GetDescription(IBusinessEntityInfoManager businessEntityManager);
        public virtual bool IsAccountIdIncluded(string accountId)
        {
            return true;
        }
    }
}
