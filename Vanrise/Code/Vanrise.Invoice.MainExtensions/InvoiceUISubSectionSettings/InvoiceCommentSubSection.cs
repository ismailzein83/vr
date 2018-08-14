using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.InvoiceUISubSectionSettings
{
    public class InvoiceCommentSubSection : InvoiceSubSectionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A6B961D2-84F3-4772-94BC-67328FCA0C05"); }
        }
    }
}
