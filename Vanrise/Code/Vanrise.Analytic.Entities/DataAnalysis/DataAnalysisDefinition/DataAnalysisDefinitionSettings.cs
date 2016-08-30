using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class DataAnalysisDefinitionSettings 
    {
        public int ConfigId { get; set; }

        public virtual List<DataAnalysisItemDefinitionConfig> ItemsConfig { get; set; }
    }


    public class DataAnalysisItemDefinitionConfig
    {
        public Guid TypeId { get; set; }

        public string Title { get; set; }

        public string Editor { get; set; }
    }
}
