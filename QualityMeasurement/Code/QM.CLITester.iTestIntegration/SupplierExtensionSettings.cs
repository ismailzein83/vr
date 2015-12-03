using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.iTestIntegration
{
    public class SupplierExtensionSettings : BusinessEntity.Entities.ExtendedSupplierSetting
    {
        public string Prefix { get; set; }

        public bool IsNew { get; set; }

        public override void Apply(BusinessEntity.Entities.Supplier supplier)
        {
            throw new NotImplementedException();
        }
    }
}
