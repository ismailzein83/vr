using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class DID
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_DID";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("674F8BE5-9F1B-4084-8EE7-4EBE6C8838AE");

        public int DIDId { get; set; }

        public string Number { get; set; }

        public DIDSettings Settings { get; set; }
    }
}
