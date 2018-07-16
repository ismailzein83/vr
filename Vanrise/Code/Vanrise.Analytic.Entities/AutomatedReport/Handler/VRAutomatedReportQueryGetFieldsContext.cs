using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportQueryGetFieldsContext : IVRAutomatedReportQueryGetFieldsContext
    {
        public Guid QueryDefinitionId { get; set; }
    }
}
