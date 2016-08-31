using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DataAnalysisItemDefinition
    {
        public Guid DataAnalysisItemDefinitionId { get; set; }

        public Guid DataAnalysisDefinitionId { get; set; }

        public string Name { get; set; }      

        public DataAnalysisItemDefinitionSettings Settings { get; set; }
    }
}
