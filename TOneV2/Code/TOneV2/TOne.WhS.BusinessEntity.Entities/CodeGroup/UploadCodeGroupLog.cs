using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class UploadCodeGroupLog
    {
        public int CountOfCodeGroupsAdded { get; set; }
        public int CountOfCodeGroupsFailed { get; set; }

        public long fileID { get; set; }
    }
}
