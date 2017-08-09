using Retail.BusinessEntity.Entities;
using Retail.MultiNet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.MultiNet.Business
{

   
    public class MultiNetBranchExtendedInfo : AccountPartSettings
    {
        public static Guid _ConfigId = new Guid("C6582B11-E9A0-4326-BF3A-DC58E36C2C8E");

        public override Guid ConfigId { get { return _ConfigId; } }

        public string BranchCode { get; set; }
        public string ContractReferenceNumber { get; set; }

        public string CNIC { get; set; }
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
        public MultiNetAccountType? AccountType { get; set; }
        public DateTime? CNICExpiryDate  { get; set; }
        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "BranchCode": return this.BranchCode;
                case "ContractReferenceNumber": return this.ContractReferenceNumber;
                case "BillingAddress": return this.BillingAddress;
                case "TechnicalAddress": return this.TechnicalAddress;
                case "OfficeAddress": return this.OfficeAddress;
                case "HomeAddress": return this.HomeAddress;
                case "CNIC": return this.CNIC;
                case "NTN": return this.NTN;
                case "PassportNumber": return this.PassportNumber;
                case "AssignedNumber": return this.AssignedNumber;
                case "PIN": return this.PIN;
                case "RefNumber": return this.RefNumber;
                case "RegistrationNumber": return this.RegistrationNumber;
                case "AccountType": return this.AccountType;
                case "CNICExpiryDate": return this.CNICExpiryDate;

                default: return null;
            }
        }

    }
}
