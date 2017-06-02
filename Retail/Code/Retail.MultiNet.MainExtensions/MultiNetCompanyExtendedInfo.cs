using Retail.BusinessEntity.Entities;
using Retail.MultiNet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.MultiNet.MainExtensions
{
    public class MultiNetCompanyExtendedInfo : AccountPartSettings
    {
        public static Guid _ConfigId = new Guid("DAD84A33-EAB3-4B9F-9EB2-392544B09727");

        public override Guid ConfigId { get { return _ConfigId; } }

        public string CNIC { get; set; }

        public string NTN { get; set; }

        public string PassportNumber { get; set; }

        public string AssignedNumber  { get; set; }

        public AddressType AddressType { get; set; }

        public string InventoryDetails { get; set; }

        public string GPSiteID { get; set; }

        public MultiNetAccountType AccountType { get; set; }

        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "CNIC": return this.CNIC;
                case "NTN": return this.NTN;
                case "PassportNumber": return this.PassportNumber;
                case "AssignedNumber": return this.AssignedNumber;
                case "AddressTypes": return this.AddressType;
                case "InventoryDetails": return this.InventoryDetails;
                case "GPSiteID": return this.GPSiteID;
                case "AccountType": return this.AccountType;
                default: return null;
            }
        }

    }
}
