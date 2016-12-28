using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ContractTypes.PrePaid
{
    public class PrePaidDefinitionSettings : ContractDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("360ADAB8-1516-4A3D-BDB7-0655C6A0965B"); }
        }
    }
}
