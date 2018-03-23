using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class UploadGenericRulesOutput
    {
        public int NumberOfGenericRulesAdded { get; set; }
        public int NumberOfGenericRulesFailed { get; set; }
        public int FileId { get; set; }
    }

}
