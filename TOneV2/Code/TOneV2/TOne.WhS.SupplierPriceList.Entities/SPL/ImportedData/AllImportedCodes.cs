using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{

    public class AllImportedCodes : IRuleTarget
    {
        public IEnumerable<ImportedCode> ImportedCodes { get; set; }

        public object Key
        {
            get { return null; }
        }
        public string TargetType
        {
            get { return "AllImportedCodes"; }
        }
    }
}
