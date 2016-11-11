using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    class SaleCodeExistingEntity : IExistingEntity
    {
        #region Public Methods

        public SaleCodeExistingEntity(BusinessEntity.Entities.SaleCode saleCode)
        {
            this.CodeEntity = saleCode;
        }

        public BusinessEntity.Entities.SaleCode CodeEntity { get; set; }

        public string ZoneName
        {
            get
            {
                return new SaleZoneManager().GetSaleZoneName(this.CodeEntity.ZoneId);
            }
        }

        public int CountryId { get; set; }

        #endregion

        #region IExistingEntity Members

        public IChangedEntity ChangedEntity
        {
            get { return null; }
        }

        public DateTime? OriginalEED
        {
            get { return this.EED; }
        }

        public bool IsSameEntity(IExistingEntity nextEntity)
        {
            SaleCodeExistingEntity nextExistingCode = nextEntity as SaleCodeExistingEntity;
            return this.ZoneName.Equals(nextExistingCode.ZoneName, StringComparison.InvariantCultureIgnoreCase);
        }

        public DateTime BED
        {
            get { return this.CodeEntity.BED; }
        }

        public DateTime? EED
        {
            get { return this.CodeEntity.EED; }
        }

        #endregion
    }
}
