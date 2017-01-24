using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductFamilyEditorRuntime
    {
        public ProductFamily Entity { get; set; }

        public Dictionary<int, string> PackageNameByIds { get; set; }
    }
}
