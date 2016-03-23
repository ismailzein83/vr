using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Entities;

namespace QM.BusinessEntity.Business
{
    public class ApplyExtendedSupplierSettingsContext : IApplyExtendedSupplierSettingsContext
    {
        public Supplier Supplier { get; set; }
    }
}
