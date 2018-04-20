using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.Entities;

namespace SOM.Main.BP.Arguments
{
    public class NewCustomerCreationProcessInput : BaseSOMRequestBPInputArg
    {
        public Guid CustomerId { get; set; }

        public override string GetTitle()
        {
            return String.Format("New Customer Creation Process Input: {0}", this.CustomerId);
        }
    }

    public class NewCustomerCreationSomRequestSetting : SOMRequestExtendedSettings
    {
        public Guid CustomerId { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("238ACF29-254E-494C-881C-F87D981A7830"); }
        }

        public override BaseSOMRequestBPInputArg ConvertToBPInputArgument(ISOMRequestConvertToBPInputArgumentContext context)
        {
            return new NewCustomerCreationProcessInput
            {
                CustomerId = this.CustomerId
            };
        }
    }
}
