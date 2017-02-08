using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Zajil.MainExtensions
{
    public class ZajilCompanyExtendedInfo : AccountPartSettings
    {
        public static Guid _ConfigId = new Guid("605BBA22-34A8-4453-86CF-167C48C99D60");

        public override Guid ConfigId { get { return _ConfigId; } }

        public string CRMCompanyId { get; set; }

        public string CRMCompanyAccountNo { get; set; }

        public string SalesAgent { get; set; }

        public string ServiceType {get ; set;}

        public string Remarks { get; set; }

        public string GPVoiceCustomerNo { get; set; }

        public string ServiceId { get; set; }

        public string CustomerPO { get; set; }

        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "CRMCompanyId": return this.CRMCompanyId;
                case "CRMCompanyAccountNo": return this.CRMCompanyAccountNo;
                case "SalesAgent": return this.SalesAgent;
                case "ServiceType": return this.ServiceType;
                case "Remarks": return this.Remarks;
                case "GPVoiceCustomerNo": return this.GPVoiceCustomerNo;
                case "ServiceId": return this.ServiceId;
                case "CustomerPO": return this.CustomerPO;
                default: return null;
            }
        }
    }
}
