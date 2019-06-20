using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Invoice.MainExtensions
{
    public abstract class GenericPartnerSettings : InvoicePartnerManager
    {
        protected GenericFinancialAccountConfiguration _configuration;
        public GenericPartnerSettings(GenericFinancialAccountConfiguration configuration)
        {
            configuration.ThrowIfNull("financialAccountMappingFieldsConfigration");
            _configuration = configuration;
        }
        public override string PartnerFilterSelector
        {
            get
            {
                return "vr-invoice-genericinvoiceaccount-selector";
            }
        }
        public override string PartnerSelector
        {
            get
            {
                return "vr-invoice-genericinvoiceaccount-selector";
            }
        }
        public override string PartnerInvoiceSettingFilterFQTN
        {
            get
            {
                return "";
            }
        }
        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            GenericFinancialAccountManager financialAccountManager = new GenericFinancialAccountManager(_configuration);
            var financialAccount = financialAccountManager.GetFinancialAccount(context.PartnerId);
            return new VRInvoiceAccountData
            {
                BED = financialAccount.BED,
                EED = financialAccount.EED,
                Status = financialAccount.Status
            };
        }
    }

}
