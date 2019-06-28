using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class ViewToAdd : View
    {
        public string ViewTypeName { get; set; }
    }
    public class UpdateViewAudiencesInput
    {
        public Guid ViewId { get; set; }
        public AudienceWrapper Audience { get; set; }
    }
    public class ViewAudiencesInfo
    {
        public Guid ViewId { get; set; }
        public string Name { get; set; }
        public AudienceWrapper Audience { get; set; }
    }

}
