﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class FieldRDLReportParameterValue : RDLCReportParameterValue
    {
        public override Guid ConfigId { get { return  new Guid("7A1A9991-EC84-4E92-B1D1-1C5140DF8FF4"); } }
        public Entities.InvoiceField Field { get; set; }

        public string FieldName { get; set; }
        public override dynamic Evaluate(IRDLCReportParameterValueContext context)
        {
            switch(this.Field)
            {
                case Entities.InvoiceField.DueDate: return context.Invoice.DueDate;
                case Entities.InvoiceField.FromDate: return context.Invoice.FromDate;
                case Entities.InvoiceField.IssueDate: return context.Invoice.IssueDate;
                case Entities.InvoiceField.Partner: return context.Invoice.PartnerId;
                case Entities.InvoiceField.SerialNumber: return context.Invoice.SerialNumber;
                case Entities.InvoiceField.ToDate: return context.Invoice.ToDate;
                case Entities.InvoiceField.CustomField: return context.Invoice.Details!=null? Vanrise.Common.Utilities.GetPropValueReader(this.FieldName).GetPropertyValue(context.Invoice.Details):null; 
            }
            return null;
        }
    }
}
