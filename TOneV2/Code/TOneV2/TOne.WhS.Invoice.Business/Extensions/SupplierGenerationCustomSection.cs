﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SupplierGenerationCustomSection : GenerationCustomSection
    {
        public override string GenerationCustomSectionDirective
        {
            get
            {
                return "whs-invoicetype-generationcustomsection-supplier";
            }
           
        }
    }
}
