using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class DataAnalysisItemDefinitionSettings
    {
        public string Title { get; set; }

        public abstract Guid ItemDefinitionTypeId { get; }
    }
}
