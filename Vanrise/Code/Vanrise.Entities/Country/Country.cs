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
    }
}