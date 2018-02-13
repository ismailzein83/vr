using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.Convertors
{
    public class InvoiceToVRObjectConvertor : TargetBEConvertor
    {
        public List<InvoicePartnerInfoObject> PartnerInfoObjects { get; set; }
        public override string Name
        {
            get
            {
                return "Invoice To VR Object Converter";
            }
        }

        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            InvoiceSourceBatch invoiceBatch = context.SourceBEBatch as InvoiceSourceBatch;
            List<ITargetBE> targetBEs = new List<ITargetBE>();
            foreach (Entities.Invoice invoice in invoiceBatch.Invoices)
            {
                try
                {
                    VRObjectsTargetBE targetBe = new VRObjectsTargetBE();
                    targetBe.TargetObjects.Add("Invoice", invoice);
                    foreach (var infoObject in PartnerInfoObjects)
                    {
                        PartnerManager partnerManager = new PartnerManager();
                        dynamic partner = partnerManager.GetPartnerInfo(invoice.InvoiceTypeId, invoice.PartnerId, infoObject.InfoType);
                        targetBe.TargetObjects.Add(infoObject.ObjectName, partner);
                    }
                    targetBEs.Add(targetBe);
                }
                catch(Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to send Invoice '{0}' due to conversion error", invoice.SerialNumber));
                    context.WriteBusinessHandledException(finalException);
                }
            }
            context.TargetBEs = targetBEs;
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {

        }
    }

    public class InvoicePartnerInfoObject
    {
        public string ObjectName { get; set; }
        public string InfoType { get; set; }
    }
}
