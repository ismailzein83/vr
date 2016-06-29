using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class UploadCountryLog
    {
        public int CountOfCountriesAdded { get; set; }
        public int CountOfCountriesExist { get; set; }

        public long fileID { get; set; }
     
    }
}
