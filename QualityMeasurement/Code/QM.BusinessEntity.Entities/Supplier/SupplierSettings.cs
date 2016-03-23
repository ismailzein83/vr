using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public class SupplierSettings
    {
        public string Prefix { get; set; }
        public Dictionary<string, Object> ExtendedSettings { get; set; }
    }

    public abstract class ExtendedSupplierSettingBehavior
    {
        public abstract void ApplyExtendedSettings(IApplyExtendedSupplierSettingsContext context);
    }

    public interface IApplyExtendedSupplierSettingsContext
    {
        Supplier Supplier { get; }
    }
}
