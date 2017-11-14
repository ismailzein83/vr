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
    public class MultiNetCompanyExtendedInfo : AccountPartSettings
    {
        public static Guid _ConfigId = new Guid("DAD84A33-EAB3-4B9F-9EB2-392544B09727");

        public override Guid ConfigId { get { return _ConfigId; } }

        public string CNIC { get; set; }

        public DateTime? CNICExpiryDate { get; set; }

        public string NTN { get; set; }
        public int BillingPeriod { get; set; }
        public DateTime? DueDate { get; set; }
        public string GPSiteID { get; set; }
        public bool ExcludeWHTaxes { get; set; }
        public bool ExcludeSaleTaxes { get; set; }
        public MultiNetAccountType? AccountType { get; set; }
        public long CustomerLogo { get; set; }
        public string AssignedNumber { get; set; }

        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "CNIC": return this.CNIC;
                case "CNICExpiryDate": return this.CNICExpiryDate;
                case "NTN": return this.NTN;
                case "GPSiteID": return this.GPSiteID;
                case "AccountType": return this.AccountType;
                case "ExcludeWHTaxes": return this.ExcludeWHTaxes;
                case "ExcludeSaleTaxes": return this.ExcludeSaleTaxes;
                case "BillingPeriod": return this.BillingPeriod;
                case "DueDate": return this.DueDate;
                case "AssignedNumber": return this.AssignedNumber;
                default: return null;
            }
        }

    }
}
