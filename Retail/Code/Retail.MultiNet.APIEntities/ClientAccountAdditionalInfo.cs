using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.APIEntities
{
    public enum MultiNetAccountType
    {
        [Description("Business Trunk")]
        BusinessTrunk = 0,
        [Description("POTS")]
        POTS = 1,
        [Description("IP Centrex")]
        IPCentrex = 2,
        [Description("Residential")]
        Residential = 3
    }
    public class ClientAccountAdditionalInfo
    {
        public MultiNetCompanyInfo MultiNetCompanyInfo { get; set; }
        public MultiNetBranchInfo MultiNetBranchInfo { get; set; }
    }
    public class MultiNetCompanyInfo
    {
        public string CNIC { get; set; }

        public DateTime? CNICExpiryDate { get; set; }

        public string NTN { get; set; }
        public string GPSiteID { get; set; }
        public bool ExcludeTaxes { get; set; }
        public string AccountType { get; set; }
        public long CustomerLogo { get; set; }
    }
    public class MultiNetBranchInfo
    {
        public string BranchCode { get; set; }
        public string ContractReferenceNumber { get; set; }

        public string CNIC { get; set; }
        public DateTime? CNICExpiryDate { get; set; }
        public string NTN { get; set; }
        public string RegistrationNumber { get; set; }
        public string RefNumber { get; set; }
        public string PassportNumber { get; set; }
        public string AssignedNumber { get; set; }
        public string PIN { get; set; }
        public string BillingAddress { get; set; }
        public string TechnicalAddress { get; set; }
        public string OfficeAddress { get; set; }
        public string HomeAddress { get; set; }
        public string AccountType { get; set; }
    }
}
