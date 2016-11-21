using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class CountryPreview
    {
        public int CountryId { get; set; }
        public int NewZones { get; set; }
        public int DeletedZones { get; set; }
        public int RenamedZones { get; set; }
        public int NewCodes { get; set; }
        public int MovedCodes { get; set; }
        public int DeletedCodes { get; set; }

    }


    public class CountryPreviewDetail
    {
        public CountryPreview Entity { get; set; }

        public string CountryName { get; set; }
    }
    
}
