using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace QM.BusinessEntity.Entities
{
    public class Supplier : Vanrise.Entities.EntitySynchronization.IItem
    {
        public int SupplierId { get; set; }

        public string Name { get; set; }

        public string SourceId { get; set; }

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
