using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{


    public class ExistingCode : IExistingEntity
    {
        public ExistingZone ParentZone { get; set; }

        public BusinessEntity.Entities.SaleCode CodeEntity { get; set; }

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

            return this.ParentZone.Name.Equals(nextExistingCode.ParentZone.Name, StringComparison.InvariantCultureIgnoreCase)
                && this.CodeEntity.Code == nextExistingCode.CodeEntity.Code;
        }
    }

    public class ExistingCodesByCodeValue : Dictionary<string, List<ExistingCode>>
    {

    }





}
