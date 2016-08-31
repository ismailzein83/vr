using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class LoadSupplierCodesInput : BaseProcessInputArgument
    {
        public int ParentWFRuntimeProcessId { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public string CodePrefix { get; set; }

        public override string GetTitle()
        {
            return string.Format("Load Supplier Codes For Prefix '{0}' sub process", this.CodePrefix);
        }
    }
}
