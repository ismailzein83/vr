﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceUISubSection
    {
        public Guid UniqueSectionID { get; set; }
        public string SectionTitle { get; set; }
        public Vanrise.GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
        public InvoiceUISubSectionSettings Settings { get; set; }
    }
    public abstract class InvoiceUISubSectionSettings
    {
        public abstract Guid ConfigId { get; }
    }
}
