using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CodeOnEachRowConstantMappedValue : CodeOnEachRowMappedValue
	{
        public override Guid ConfigId
        {
            get { return new Guid("A3558AE8-A64C-4A1B-B1D3-81665720992A"); }
        }

		public string Value { get; set; }

        public override void Execute(ICodeOnEachRowMappedValueContext context)
		{
			context.Value = Value;
		}
        
    }
}
