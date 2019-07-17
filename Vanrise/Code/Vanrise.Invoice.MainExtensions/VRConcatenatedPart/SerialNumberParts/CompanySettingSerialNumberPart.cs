using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Extensions;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts
{
    public class CompanySettingSerialNumberPart : VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return new Guid("8B4B4603-615E-4B02-8113-46CB71AE156A"); } }
        public string InfoType { get; set; }
        public override string GetPartText(IInvoiceSerialNumberConcatenatedPartContext context)
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            if (context != null & context.InvoiceTypeId != null)
            {
                var invoiceType = invoiceTypeManager.GetInvoiceType(context.InvoiceTypeId);
                if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.ExtendedSettings != null)
                {
                    var info = invoiceType.Settings.ExtendedSettings.GetInfo(new InvoiceTypeExtendedSettingsInfoContext { InfoType = InfoType, Invoice = context.Invoice });
                    var companySetting = (CompanySetting)info;

                    if (companySetting != null)
                        return companySetting.CompanyName;
                }
            }
            return null;
        }
    }
}
