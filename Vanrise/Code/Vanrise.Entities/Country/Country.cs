using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class Country : EntitySynchronization.IItem
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "VR_Common_Country";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("DF5CDC08-DDF1-4D4E-B1F6-D17B3833452F");

        public int CountryId { get; set; }

        public string Name { get; set; }

        public string SourceId { get; set; }


        public long ItemId
        {
            get
            {
                return this.CountryId;
            }
            set
            {
                this.CountryId = (int)value;
            }
        }

        public DateTime? CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
    }
}