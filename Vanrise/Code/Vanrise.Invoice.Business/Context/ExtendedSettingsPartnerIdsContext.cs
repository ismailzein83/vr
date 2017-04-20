﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class ExtendedSettingsPartnerIdsContext : IExtendedSettingsPartnerIdsContext
    {
        public Guid InvoiceTypeId { get; set; }
        public PartnerRetrievalType PartnerRetrievalType { get; set; }
    }
}
