﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public interface IRulePhoneNumberLengthCriteria
    {
        IEnumerable<int> PhoneNumberLengths { get; }
    }
}
