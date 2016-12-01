using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
  public abstract  class CodeOnEachRowMappedValue
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(ICodeOnEachRowMappedValueContext context);
    }
}
