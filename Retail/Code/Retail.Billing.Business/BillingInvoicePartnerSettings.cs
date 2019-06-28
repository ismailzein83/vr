using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;

namespace Retail.Billing.Business
{
    public enum RDLCParameter
    {
        CustomerName = 0,
        Region = 1,
        City = 2,
        Town = 3,
        Street = 4,
        Building = 5,
        FloorNumber = 6,
        AddressNotes = 7,
        FullName = 8,
        FinancialAccountId = 9
    }

    public class BillingInvoicePartnerSettings : GenericPartnerSettings
    {
        public BillingInvoicePartnerSettings(GenericFinancialAccountConfiguration configuration) : base(configuration)
        {
        }

        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            return Convert.ToInt32(context.PartnerId);
        }

        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            GenericFinancialAccountManager financialAccountManager = new GenericFinancialAccountManager(this._configuration);
            var financialAccount = financialAccountManager.GetFinancialAccount(context.PartnerId);
            switch (context.InfoType)
            {
                case "Account":
                    return financialAccount;
                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();

                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.CustomerName, financialAccount.Name, true);
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.FinancialAccountId, financialAccount.FinancialAccountId.PadLeft(8, '0'), true);

                    if (financialAccount.ExtraFields != null)
                    {
                        var regionId = financialAccount.ExtraFields.GetRecord("Region");
                        var regionDescription = regionId != null ? financialAccountManager.GetExtraFieldValueDescription("Region", regionId) : "";
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Region, regionDescription, true);

                        var cityId = financialAccount.ExtraFields.GetRecord("City");
                        var cityDescription = cityId != null ? financialAccountManager.GetExtraFieldValueDescription("City", cityId) : "";
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.City, cityDescription, true);

                        var townId = financialAccount.ExtraFields.GetRecord("Town");
                        var townDescription = townId != null ? financialAccountManager.GetExtraFieldValueDescription("Town", townId) : "";
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Town, townDescription, true);

                        var street = financialAccount.ExtraFields.GetRecord("Street");
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Street, street != null ? street.ToString() : "", true);

                        var building = financialAccount.ExtraFields.GetRecord("Building");
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Building, building != null ? building.ToString() : "", true);

                        var floorNumber = financialAccount.ExtraFields.GetRecord("FloorNumber");
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.FloorNumber, floorNumber != null ? floorNumber.ToString() : "", true);

                        var addressNotes = financialAccount.ExtraFields.GetRecord("AddressNotes");
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.AddressNotes, addressNotes != null ? addressNotes.ToString() : "", true);
                    }

                    return rdlcReportParameters;
            }
            return null;

        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            GenericFinancialAccountManager financialAccountManager = new GenericFinancialAccountManager(this._configuration);
            var financialAccount = financialAccountManager.GetFinancialAccount(context.PartnerId);
            return financialAccount.Name;
        }

        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }

    }
}
