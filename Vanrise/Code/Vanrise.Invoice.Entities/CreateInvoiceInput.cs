﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class CreateInvoiceInput
    {
        string PartnerId { get; set; }

        DateTime FromDate { get; set; }

        DateTime ToDate { get; set; }
    }
}
