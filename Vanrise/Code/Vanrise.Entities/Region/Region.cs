using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class Region
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "VR_Common_Region";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("878D74E5-4325-4A70-A247-9067798837FA");

        public int RegionId { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public RegionSettings Settings { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
        
    }

    public class RegionSettings
    {

    }
}
