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

        public string ServiceType {get ; set;}

        public string Remarks { get; set; }

        public string GPVoiceCustomerNo { get; set; }

        public string ServiceId { get; set; }
    }
}
