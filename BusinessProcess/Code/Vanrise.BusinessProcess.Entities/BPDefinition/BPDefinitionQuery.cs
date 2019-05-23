using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPDefinitionQuery
    {
        public string Title { get; set; }
        public bool? ShowOnlyVisibleInManagementScreen { get; set; }
        public List<Guid> DevProjectIds { get; set; }

    }
}