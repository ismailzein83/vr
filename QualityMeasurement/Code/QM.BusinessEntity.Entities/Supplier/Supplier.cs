using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public class Supplier : IItem
    {
        public int SupplierId { get; set; }

        public string Name { get; set; }

        public SupplierSettings Settings { get; set; }

        long IItem.ItemId
        {
            get
            {
                return SupplierId;
            }
            set
            {
                this.SupplierId = (int)value;
            }
        }
    }
}
