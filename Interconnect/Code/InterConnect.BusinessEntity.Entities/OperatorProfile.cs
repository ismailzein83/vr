using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterConnect.BusinessEntity.Entities
{
    public class OperatorProfile
    {
        public int OperatorProfileId { get; set; }

        public string Name { get; set; }

        public OperatorProfileSettings Settings { get; set; }

        public int? ExtendedSettingsRecordTypeId { get; set; }

        public dynamic ExtendedSettings { get; set; }
    }
}
