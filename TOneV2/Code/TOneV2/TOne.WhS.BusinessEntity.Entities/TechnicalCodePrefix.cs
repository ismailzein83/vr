using System.Collections.Generic;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class TechnicalCodePrefix : ICode
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public int ZoneID { get; set; }
    }
}
