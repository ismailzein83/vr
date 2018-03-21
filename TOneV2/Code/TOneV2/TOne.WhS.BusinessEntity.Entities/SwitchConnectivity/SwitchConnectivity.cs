using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BaseSwitchConnectivity
    {
        public int SwitchConnectivityId { get; set; }

        public string Name { get; set; }

        public int CarrierAccountId { get; set; }

        public int SwitchId { get; set; }

        public SwitchConnectivitySettings Settings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }


    }

    public class SwitchConnectivity : BaseSwitchConnectivity
    {
        public string SourceId { get; set; }
    }

    public class SwitchConnectivityToEdit : BaseSwitchConnectivity
    {

    }
}
