using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.Convertors
{
    public class InvoiceToVRObjectConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "Invoice To VR Object Convertor";
            }
        }

        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            InvoiceSourceBatch invoiceBatch = context.SourceBEBatch as InvoiceSourceBatch;
            List<ITargetBE> targetBEs = new List<ITargetBE>();
            foreach (Entities.Invoice invoice in invoiceBatch.Invoices)
            {
                InvoiceTargetBE targetBe = new InvoiceTargetBE();
                targetBe.TargetObjects.Add("Invoice", invoice);
                targetBEs.Add(targetBe);
            }
            context.TargetBEs = targetBEs;
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {

        }
    }
}
