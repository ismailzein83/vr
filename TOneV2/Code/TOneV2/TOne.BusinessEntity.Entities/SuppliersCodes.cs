using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class SuppliersCodes
    {
        /// <summary>
        /// Key is SupplierID, Value is Supplier Codes
        /// </summary>
        public Dictionary<string, SupplierCodes> Codes { get; set; }
    }
}
