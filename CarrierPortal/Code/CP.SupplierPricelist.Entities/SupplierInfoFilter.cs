using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class SupplierInfoFilter
    {
        /// <summary>
        /// use this in case of current Customer User
        /// </summary>
        public int? AssignableToSupplierUserId { get; set; }

        /// <summary>
        /// use this to get accounts of current Supplier User for selected Customer
        /// </summary>
        public int? CustomerIdForCurrentSupplier { get; set; }
    }
}
