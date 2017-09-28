using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListEvaluatedEmail
    {
        public long FileId { get; set; }
        public VRMailEvaluatedTemplate EvaluatedTemplate { get; set; }
        public IEnumerable<SalePricelistVRFile> SalePricelistVrFiles { get; set; }
    }
}
