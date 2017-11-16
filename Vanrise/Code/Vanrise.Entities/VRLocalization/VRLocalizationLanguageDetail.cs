using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRLocalizationLanguageDetail
    {
        public Guid VRLanguageId { get; set; }

        public string Name { get; set; }

        public Guid? ParentLanguageId { get; set; }
    }
}
