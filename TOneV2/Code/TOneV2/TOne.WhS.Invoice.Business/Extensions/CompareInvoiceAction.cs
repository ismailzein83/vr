﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CompareInvoiceAction : InvoiceActionSettings
    {
        public override string ActionTypeName { get { return "CompareInvoiceAction"; } }
        public override Guid ConfigId { get { return new Guid("CA8088B9-2887-4B85-B263-49C895088E66"); } }
        public Guid ItemGroupingId { get; set; }
        public Guid ZoneDimensionId { get; set; }
        public Guid FromDateDimensionId { get; set; }
        public Guid ToDateDimensionId { get; set; }
        public Guid CurrencyDimensionId { get; set; }
        public Guid RateDimensionId { get; set; }
        public Guid RateTypeDimensionId { get; set; }



        public Guid NumberOfCallsMeasureId { get; set; }
        public Guid AmountMeasureId { get; set; }
        public Guid DurationMeasureId { get; set; }

        public string PartnerLabel { get; set; }
        public string PartnerAbbreviationLabel { get; set; }
    }
}
