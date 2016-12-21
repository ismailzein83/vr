using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountGridColumnAttribute
    {
        public string FieldName { get; set; }

        public string Header { get; set; }

        public string Type { get; set; }

        public Object Field { get; set; }
    }
}
