using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DataAnalysisDefinitionConfig
    {
        public string Editor { get; set; }

        public List<DataAnalysisItemDefinitionConfig> ItemsConfig { get; set; }
    }

    public class DataAnalysisItemDefinitionConfig
    {
        public Guid TypeId { get; set; }

        public string Title { get; set; }

        public string Editor { get; set; }
    }
}
