﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceTypeDataManager:IDataManager
    {
        List<InvoiceType> GetInvoiceTypes();
        bool AreInvoiceTypesUpdated(ref object updateHandle);

    }
}
