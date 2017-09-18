using Retail.BusinessEntity.Business;
using Retail.MultiNet.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.MultiNet.Business
{
    public class MultiNetAccountManager
    {
        public ClientAccountAdditionalInfo GetClientAccountAdditionalInfo(Guid accountBEDefinitionId, long accountId)
        {
            var account = new AccountBEManager().GetAccount(accountBEDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            ClientAccountAdditionalInfo clientAccountAdditionalInfo = null;
            if(account.Settings != null && account.Settings.Parts != null)
            {
                clientAccountAdditionalInfo = new ClientAccountAdditionalInfo();
                foreach(var part in account.Settings.Parts)
                {
                    MultiNetCompanyExtendedInfo companyInfo = part.Value.Settings as MultiNetCompanyExtendedInfo;
                    if(companyInfo != null)
                    {
                        clientAccountAdditionalInfo.MultiNetCompanyInfo = new MultiNetCompanyInfo
                        {
                            AccountType = companyInfo.AccountType.HasValue ? Utilities.GetEnumDescription(companyInfo.AccountType.Value) : null,
                            CNIC = companyInfo.CNIC,
                            GPSiteID = companyInfo.GPSiteID,
                            CNICExpiryDate = companyInfo.CNICExpiryDate,
                            CustomerLogo = companyInfo.CustomerLogo,
                            ExcludeTaxes = companyInfo.ExcludeTaxes,
                            NTN = companyInfo.NTN,
                        };
                        break;
                    }
                    MultiNetBranchExtendedInfo branchInfo = part.Value.Settings as MultiNetBranchExtendedInfo;
                    if (branchInfo != null)
                    {
                        clientAccountAdditionalInfo.MultiNetBranchInfo = new MultiNetBranchInfo
                        {
                            AccountType = branchInfo.AccountType.HasValue? Utilities.GetEnumDescription(branchInfo.AccountType.Value):null,
                            AssignedNumber = branchInfo.AssignedNumber,
                            BillingAddress = branchInfo.BillingAddress,
                            BranchCode = branchInfo.BranchCode,
                            CNIC = branchInfo.CNIC,
                            CNICExpiryDate = branchInfo.CNICExpiryDate,
                            ContractReferenceNumber = branchInfo.ContractReferenceNumber,
                            HomeAddress = branchInfo.HomeAddress,
                            NTN = branchInfo.NTN,
                            OfficeAddress = branchInfo.OfficeAddress,
                            PassportNumber = branchInfo.PassportNumber,
                            PIN = branchInfo.PIN,
                            RefNumber = branchInfo.RefNumber,
                            RegistrationNumber = branchInfo.RegistrationNumber,
                            TechnicalAddress = branchInfo.TechnicalAddress,
                        };
                        break;
                    }
                }
            }
            return clientAccountAdditionalInfo;
        }
    }
}
