using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class OverriddenConfiguration
    {
        public Guid OverriddenConfigurationId { get; set; }

        public string Name { get; set; }

        public Guid GroupId { get; set; }

        public OverriddenConfigurationSettings Settings { get; set; }
    }

    public class OverriddenConfigurationDetail
    {
        public Guid OverriddenConfigurationId { get; set; }

        public string Name { get; set; }

        public string OverriddenConfigurationGroupName { get; set; }
    }
}
