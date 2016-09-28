using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class SummaryTransformationDefinitionInfoFilter
    {
        public Guid? RawItemRecordTypeId { get; set; }

        public Guid? SummaryItemRecordTypeId { get; set; }
    }
}
