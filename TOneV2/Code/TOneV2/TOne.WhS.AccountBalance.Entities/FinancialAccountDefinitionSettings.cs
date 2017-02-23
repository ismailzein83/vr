using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Entities
{
    public class FinancialAccountDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("0144FF10-50C7-4B62-9C3A-62E7B0F2364C"); }
        }

        public FinancialAccountDefinitionExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class FinancialAccountDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool IsApplicableToCustomer { get; }

        public abstract bool IsApplicableToSupplier { get; }

        public virtual string RuntimeEditor { get; set; }
    }
}
