using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Entities;

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
            bool getAccounts = true;

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
            return LoadInvoiceCarrier(getProfiles, getAccounts,filter.GetCustomers,filter.GetSuppliers);
        }
        public List<InvoiceCarrier> LoadInvoiceCarrier(bool getProfiles, bool getAccounts, bool getCustomers, bool getSuppliers)
        {
            List<InvoiceCarrier> invoiceCarriers = new List<InvoiceCarrier>();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            var carrierProfiles = carrierProfileManager.GetCarrierProfiles();
            foreach (var carrierProfile in carrierProfiles)
            {
                if (carrierProfile.Settings.CustomerByProfile)
                {
                    if(getProfiles)
                    {
                    invoiceCarriers.Add(new InvoiceCarrier
                    {
                        InvoiceCarrierId = string.Format("Profile_{0}", carrierProfile.CarrierProfileId),
                        Name = carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId)
                    });
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
                                invoiceCarriers.Add(new InvoiceCarrier
                                {
                                    InvoiceCarrierId = string.Format("Account_{0}", account.CarrierAccountId),
                                    Name = carrierAccountManager.GetCarrierAccountName(account.CarrierAccountId)
                                });
                            }
                        }
                    }
                }
            }
            return invoiceCarriers;
        }
    }
}
