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
    public class ImportedZoneServiceGroup : Vanrise.Entities.IDateEffectiveSettings,IExclude
    {
        public List<int> ServiceIds { get; set; }
        public string ZoneName { get; set; }
        public SystemZoneServiceGroup SystemZoneServiceGroup { get; set; }
        public ZoneServiceChangeType ChangeType { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

        #region IExclude Implementation
        public bool IsExcluded { get; set; }
        public void SetAsExcluded()
        {
            IsExcluded = true;

            foreach (var newZoneService in NewZoneServices)
                newZoneService.IsExcluded = true;

            foreach (var changedExistingZoneService in ChangedExistingZoneServices)
            {
                if (changedExistingZoneService.ChangedZoneService != null)
                    changedExistingZoneService.ChangedZoneService.IsExcluded = true;
            }
        }
        #endregion

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

    }
}
