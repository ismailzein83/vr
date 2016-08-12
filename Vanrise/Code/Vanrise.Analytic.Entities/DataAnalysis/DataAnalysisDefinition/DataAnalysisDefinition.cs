using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DataAnalysisDefinition
    {
        public Guid DataAnalysisDefinitionId { get; set; }

        public string Name { get; set; }

        public DataAnalysisDefinitionSettings Settings { get; set; }
    }
}
