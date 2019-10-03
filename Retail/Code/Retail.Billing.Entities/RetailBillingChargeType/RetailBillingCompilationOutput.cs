using System;
using System.Collections.Generic;

namespace Retail.Billing.Entities
{
    public class RetailBillingCompilationOutput
    {
            public List<string> ErrorMessages { get; set; }
            public bool Result { get; set; }
    }
}
