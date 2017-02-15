using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class InvoiceManager
    {
        public IEnumerable<InvoiceCarrier> GetInvoiceCarriers(InvoiceCarrierFilter filter)
        {
            List<InvoiceCarrier> invoiceCarriers = new List<InvoiceCarrier>();
            if (filter == null)
                throw new NullReferenceException("filter");
            bool getProfiles = false;
            bool getAccounts = false;

            if(filter.CarrierTypes == null)
            {
                getProfiles = true;
                getAccounts = true; 
            }else
            {
                foreach(var carrierType in filter.CarrierTypes)
                {
                    switch(carrierType)
                    {
                        case InvoiceCarrierType.Profile: getProfiles = true; break;
                        case InvoiceCarrierType.Account: getAccounts = true; break;
                    }
                }
            }
            return LoadInvoiceCarrier(getProfiles, getAccounts,filter.GetCustomers,filter.GetSuppliers,filter.Filters);
        }
        public List<InvoiceCarrier> LoadInvoiceCarrier(bool getProfiles, bool getAccounts, bool getCustomers, bool getSuppliers,List<IInvoicePartnerFilter> filters)
        {
            List<InvoiceCarrier> invoiceCarriers = new List<InvoiceCarrier>();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            var carrierProfiles = carrierProfileManager.GetCarrierProfiles();
            foreach (var carrierProfile in carrierProfiles)
            {
                if (carrierProfile.Settings.CustomerInvoiceByProfile)
                {
                    if (getProfiles)
                    {
                        var invoiceCarrierId = string.Format("Profile_{0}", carrierProfile.CarrierProfileId);
                        if (CheckIfPartnerMatched(filters, invoiceCarrierId))
                        {

                            var invoiceCarrier = new InvoiceCarrier
                            {
                                InvoiceCarrierId = invoiceCarrierId,
                                Name = carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId),
                            };
                            if (carrierProfile.Settings.CustomerInvoiceTimeZone)
                            {
                                invoiceCarrier.TimeZoneId = carrierProfile.Settings.DefaultCusotmerTimeZoneId;
                            }
                            invoiceCarriers.Add(invoiceCarrier);
                        }
                    }

                }
                else
                {
                    if (getAccounts)
                    {
                        var accounts = carrierAccountManager.GetCarriersByProfileId(carrierProfile.CarrierProfileId, getCustomers, getSuppliers);
                        if (accounts != null)
                        {
                            foreach (var account in accounts)
                            {
                                var invoiceCarrierId = string.Format("Account_{0}", account.CarrierAccountId); 
                                if (CheckIfPartnerMatched(filters, invoiceCarrierId))
                                {
                                    var invoiceCarrier = new InvoiceCarrier
                                    {
                                        InvoiceCarrierId = invoiceCarrierId,
                                        Name = carrierAccountManager.GetCarrierAccountName(account.CarrierAccountId),
                                    };
                                    if(account.CustomerSettings.InvoiceTimeZone && account.CustomerSettings.TimeZoneId.HasValue)
                                    {
                                        invoiceCarrier.TimeZoneId = account.CustomerSettings.TimeZoneId.Value;

                                    }else if(carrierProfile.Settings.CustomerInvoiceTimeZone)
                                    {
                                       invoiceCarrier.TimeZoneId = carrierProfile.Settings.DefaultCusotmerTimeZoneId;
                                    }
                                    invoiceCarriers.Add(invoiceCarrier);
                                }
                            }
                        }
                    }
                }
            }
            return invoiceCarriers;
        }
        bool CheckIfPartnerMatched(List<IInvoicePartnerFilter> filters,string partnerId)
        {
            if(filters != null)
            {
                InvoicePartnerFilterContext context = new InvoicePartnerFilterContext{
                    PartnerId = partnerId
                };
                foreach(var filter in filters)
                {
                    if (!filter.IsMatched(context))
                        return false;
                }
            }
            return true;
        }
    }
}
