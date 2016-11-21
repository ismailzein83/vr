using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public class CodePreview
    {
        public string Code { get; set; }

        public CodeChangeType ChangeType { get; set; }

        public string RecentZoneName { get; set; }

        public string ZoneName { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class CodePreviewDetail
    {
        public CodePreview Entity { get; set; }

        public string ChangeTypeDecription { get; set; }
    }
}
