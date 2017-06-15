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


        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "BranchCode": return this.BranchCode;
                case "ContractReferenceNumber": return this.ContractReferenceNumber;
                default: return null;
            }
        }

    }
}
