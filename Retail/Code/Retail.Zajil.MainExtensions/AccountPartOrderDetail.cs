using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Zajil.MainExtensions
{
    public class AccountPartOrderDetail : AccountPartSettings
    {
        public static Guid _ConfigId = new Guid("B272B8B9-0501-4322-A4AD-360FDF5D933D");
        public override Guid ConfigId { get { return _ConfigId; } }
        public List<OrderDetailItem> OrderDetailItems { get; set; }
    }
}
