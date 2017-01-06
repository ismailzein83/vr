using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ProductTypes.PrePaid
{
    public class PrePaidDefinitionSettings : ProductDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("360ADAB8-1516-4A3D-BDB7-0655C6A0965B"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-productextendedsettings-prepaid";
            }
        }
    }
}
