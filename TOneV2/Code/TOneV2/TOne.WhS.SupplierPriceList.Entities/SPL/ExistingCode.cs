using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ExistingCode : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget
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


        public object Key
        {
            get { return this.CodeEntity.Code; }
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
