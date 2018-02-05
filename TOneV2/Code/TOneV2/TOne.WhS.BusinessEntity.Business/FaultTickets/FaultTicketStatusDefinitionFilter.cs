using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class FaultTicketStatusDefinitionFilter : IStatusDefinitionFilter
    {
        public long? CaseId { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public bool IsMatched(IStatusDefinitionFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
