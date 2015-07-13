using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class ViewContent
    {
        public List<Content> SummaryContents { get; set; }
        public List<Content> BodyContents { get; set; }
        public int DefaultGrouping { get; set; }
        public int DefaultPeriod { get; set; }
    }
}
