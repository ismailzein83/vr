using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CodesByZoneConstantMappedValue : CodesByZoneMappedValue
	{
        public override Guid ConfigId
        {
            get { return new Guid("EC196547-7AC1-4C6D-9B6F-7A7404DC5102"); }
        }

		public string Value { get; set; }

        public override void Execute(ICodesByZoneMappedValueContext context)
        {
            context.Value = Value;
        }
    }
}
