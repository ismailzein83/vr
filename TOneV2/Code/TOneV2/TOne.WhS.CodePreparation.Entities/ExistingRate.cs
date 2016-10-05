using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ExistingRate : IExistingEntity
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SaleRate RateEntity { get; set; }

        public ChangedRate ChangedRate { get; set; }

        public IChangedEntity ChangedEntity
        {
            get { return this.ChangedRate; }
        }

        public DateTime BED
        {
            get { return RateEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedRate != null ? ChangedRate.EED : RateEntity.EED; }
        }

        public bool IsSameEntity(IExistingEntity nextEntity)
        {
            ExistingRate nextExistingRate = nextEntity as ExistingRate;

            return this.ParentZone.Name.Equals(nextExistingRate.ParentZone.Name, StringComparison.InvariantCultureIgnoreCase)
                && this.RateEntity.NormalRate == nextExistingRate.RateEntity.NormalRate;
        }


        public DateTime? OriginalEED
        {
            get { return this.RateEntity.EED; }
        }
    }
}
