using Retail.BusinessEntity.APIEntities;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Retail.BusinessEntity.MainExtensions
{
    public class RetailClientAccountManager
    {
        public ClientRetailProfileAccountInfo GetClientProfileAccountInfo(Guid accountBEDefinitionId, long accountId)
        {
            var accountBEManager = new AccountBEManager();
            var account = accountBEManager.GetAccount(accountBEDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            ClientRetailProfileAccountInfo clientRetailProfileAccountInfo = null;
            if(account.Settings != null && account.Settings.Parts != null)
            {
                clientRetailProfileAccountInfo = new ClientRetailProfileAccountInfo();
                CountryManager countryManager = new CountryManager();
                CityManager cityManager = new CityManager();
                CurrencyManager currencyManager = new CurrencyManager();
                ProductManager productManager =new ProductManager();

                foreach(var part in account.Settings.Parts )
                {
                    AccountPartCompanyProfile accountPartCompanyProfile =  part.Value.Settings as AccountPartCompanyProfile;

                    if(accountPartCompanyProfile != null)
                    {
                        clientRetailProfileAccountInfo.PhoneNumbers = accountPartCompanyProfile.PhoneNumbers;
                        clientRetailProfileAccountInfo.PhoneNumbersDescription = accountPartCompanyProfile.PhoneNumbers != null ? string.Join(",", accountPartCompanyProfile.PhoneNumbers) : null;
                        clientRetailProfileAccountInfo.MobileNumbers = accountPartCompanyProfile.MobileNumbers;
                        clientRetailProfileAccountInfo.MobileNumbersDescription = accountPartCompanyProfile.MobileNumbers != null ? string.Join(",", accountPartCompanyProfile.MobileNumbers) : null;
                        clientRetailProfileAccountInfo.Faxes = accountPartCompanyProfile.Faxes;
                        clientRetailProfileAccountInfo.FaxesDescription = accountPartCompanyProfile.Faxes != null ? string.Join(",", accountPartCompanyProfile.Faxes) : null;
                        clientRetailProfileAccountInfo.Address = accountPartCompanyProfile.Address;


                        clientRetailProfileAccountInfo.ArabicName = accountPartCompanyProfile.ArabicName;
                        clientRetailProfileAccountInfo.CityId = accountPartCompanyProfile.CityId;
                        if (clientRetailProfileAccountInfo.CityId.HasValue)
                          clientRetailProfileAccountInfo.CityName = cityManager.GetCityName(clientRetailProfileAccountInfo.CityId.Value);

                        clientRetailProfileAccountInfo.CountryId = accountPartCompanyProfile.CountryId;
                        if (clientRetailProfileAccountInfo.CountryId.HasValue)
                            clientRetailProfileAccountInfo.CountryName = countryManager.GetCountryName(clientRetailProfileAccountInfo.CountryId.Value);

                        clientRetailProfileAccountInfo.POBox = accountPartCompanyProfile.POBox;
                        clientRetailProfileAccountInfo.Street = accountPartCompanyProfile.Street;
                        clientRetailProfileAccountInfo.Town = accountPartCompanyProfile.Town;
                        clientRetailProfileAccountInfo.Website = accountPartCompanyProfile.Website;
                        clientRetailProfileAccountInfo.AccountName = accountBEManager.GetAccountName(account);
                        if (accountPartCompanyProfile.Contacts != null)
                        {
                            clientRetailProfileAccountInfo.Contacts = new List<APIEntities.AccountCompanyContact>();
                            foreach(var contact in accountPartCompanyProfile.Contacts)
                            {
                                string contactName = "";
                                if (contact.Value.Salutation.HasValue)
                                {
                                    contactName = string.Format("{0}. ", Utilities.GetEnumDescription(contact.Value.Salutation.Value));
                                }
                                contactName = string.Format("{0}{1}",contactName, contact.Value.ContactName);
                                clientRetailProfileAccountInfo.Contacts.Add(new APIEntities.AccountCompanyContact
                                {
                                    ContactName = contactName,
                                    Email = contact.Value.Email,
                                    Title = contact.Value.Title,
                                    Salutation = contact.Value.Salutation,
                                    MobileNumbers = contact.Value.MobileNumbers,
                                    Notes = contact.Value.Notes,
                                    PhoneNumbers = contact.Value.PhoneNumbers,
                                    PhoneNumbersDescription = contact.Value.PhoneNumbers != null?string.Join(",",contact.Value.PhoneNumbers):null,
                                    MobileNumbersDescription = contact.Value.MobileNumbers != null ? string.Join(",", contact.Value.MobileNumbers) : null,
                                    SalutationDescription =contact.Value.Salutation.HasValue? Utilities.GetEnumDescription(contact.Value.Salutation.Value):null,
                                });
                            }
                        }
                        continue;
                    }
                    AccountPartFinancial accountPartFinancial = part.Value.Settings as AccountPartFinancial;
                    if (accountPartFinancial != null)
                    {
                        clientRetailProfileAccountInfo.CurrencyId = accountPartFinancial.CurrencyId;
                        clientRetailProfileAccountInfo.CurrencyName = currencyManager.GetCurrencySymbol(clientRetailProfileAccountInfo.CurrencyId );
                        clientRetailProfileAccountInfo.ProductId = accountPartFinancial.ProductId;
                        clientRetailProfileAccountInfo.ProductName = productManager.GetProductName(clientRetailProfileAccountInfo.ProductId);
                        continue;
                    }
                }
                return clientRetailProfileAccountInfo;
            }
            return clientRetailProfileAccountInfo;
        }
    }
}
