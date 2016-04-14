using System;
using System.Collections.Generic;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingCode : ICode
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SupplierCode CodeEntity { get; set; }

        public ChangedCode ChangedCode { get; set; }

        public DateTime BED
        {
            get { return CodeEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedCode != null ? ChangedCode.EED : CodeEntity.EED; }
        }

        public string Code { get; set; }

        public object Key
        {
            get { return CodeEntity.Code; }
        }

        public void SetExcluded()
        {
            throw new NotImplementedException();
        }

        public string TargetType
        {
            get { return "Code"; }
        }
    }
    public class ExistingCodesByCodeValue : Dictionary<string, List<ExistingCode>>
    {

    }
}
