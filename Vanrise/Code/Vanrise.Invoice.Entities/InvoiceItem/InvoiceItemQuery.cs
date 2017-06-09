﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceItemQuery
    {
        public long InvoiceId { get; set; }
        public dynamic InvoiceItemDetails { get; set; }
        public string ItemSetName { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public Guid UniqueSectionID { get; set; }
        public CompareOperator CompareOperator { get; set; }
       // public List<InvoiceSubSectionGridColumn> GridColumns { get; set; }
        public List<InvoiceItemConcatenatedPart> ItemSetNameParts { get; set; }
    }
}
