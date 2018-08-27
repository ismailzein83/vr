using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.MultiNet.Entities
{
    public class FaultTicketStatusDefinitionFilter : IStatusDefinitionFilter
    {
        public Guid? StatusId { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public bool IsMatched(IStatusDefinitionFilterContext context)
        {
            var openStatusId = new Guid("dc0b6f51-0805-4130-8c03-ac8baeea2539");
            var closedStatusId = new Guid("3555f134-e88b-4520-b812-d72fcf2eb9df");
            var pendingStatusId = new Guid("d2abf349-6c3a-4333-b4da-19ece7df5d29");

            if (StatusId.HasValue)
            {
                if (StatusId.Value == context.StatusDefinition.StatusDefinitionId)
                    return true;

                if (StatusId.Value == openStatusId)
                {
                    if (context.StatusDefinition.StatusDefinitionId == closedStatusId || context.StatusDefinition.StatusDefinitionId == pendingStatusId)
                        return true;
                }
                else if (StatusId.Value == pendingStatusId)
                {
                    if (context.StatusDefinition.StatusDefinitionId == closedStatusId)
                        return true;
                }
            }
            else
            {
                if (context.StatusDefinition.StatusDefinitionId == openStatusId)
                    return true;
            }
            return false;
        }
    }
}
