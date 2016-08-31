using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DataAnalysisItemDefinitionQuery
    {
        public Guid DataAnalysisDefinitionId { get; set; }

        public Guid ItemDefinitionTypeId { get; set; }
    }
}
