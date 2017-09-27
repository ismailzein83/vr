using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricelistVRFile
    {
        public long FileId { get; set; }
        public string FileName { get; set; }
        public string CurrencySymbol { get; set; }
        public string FileExtension { get; set; }
    }
}
