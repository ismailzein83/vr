using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace Vanrise.Entities
{
    public class Currency 
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "VR_Common_Currency";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("D41EA344-C3C0-4203-8583-019B6B3EDB76");

        public int CurrencyId { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public string SourceId { get; set; }

        public DateTime? CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
        
    }

    public class CurrencyDetail
    {
        public Currency Entity { get; set; }

        public bool? IsMain { get; set; }



    }
}
