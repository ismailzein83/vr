using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DownloadUploadTemplateInput
    {
        public List<string> CriteriaFieldsToHide { get; set; }

        public Guid RuleDefinitionId { get; set; }
    }
}
