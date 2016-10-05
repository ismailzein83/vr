using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingCode : IExistingEntity
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierCode CodeEntity { get; set; }

        public ChangedCode ChangedCode { get; set; }

        public IChangedEntity ChangedEntity
        {
            get { return this.ChangedCode; }
        }

        public DateTime BED
        {
            get { return CodeEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedCode != null ? ChangedCode.EED : CodeEntity.EED; }
        }

        public bool IsSameEntity(IExistingEntity nextEntity)
        {
            ExistingCode nextExistingCode = nextEntity as ExistingCode;

            return this.ParentZone.Name.Equals(nextExistingCode.ParentZone.Name, StringComparison.InvariantCultureIgnoreCase);
        }


        public DateTime? OriginalEED
        {
            get { return this.CodeEntity.EED; }
        }
    }
    public class ExistingCodesByCodeValue : Dictionary<string, List<ExistingCode>>
    {

    }
}
