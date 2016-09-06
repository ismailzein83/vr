using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedZoneService : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget
    {
        public List<int> ServiceIds { get; set; }
        public string ZoneName { get; set; }

        public ZoneServiceChangeType ChangeType { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

        List<NewZoneService> _newZoneServices = new List<NewZoneService>();
        public List<NewZoneService> NewZoneServices
        {
            get
            {
                return _newZoneServices;
            }
        }

        List<ExistingZoneService> _changedExistingZoneServices = new List<ExistingZoneService>();
        public List<ExistingZoneService> ChangedExistingZoneServices
        {
            get
            {
                return _changedExistingZoneServices;
            }
        }

        #region IRuleTarget Implementation

        public object Key
        {
            get { return this.ZoneName; }
        }


        #endregion


        public string TargetType
        {
            get { return "ZoneService"; }
        }



    }
}
