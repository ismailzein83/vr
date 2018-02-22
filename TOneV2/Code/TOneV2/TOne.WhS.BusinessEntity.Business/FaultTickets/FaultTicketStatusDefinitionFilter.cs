using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class FaultTicketStatusDefinitionFilter : IStatusDefinitionFilter
    {
        public long? CaseId { get; set; }
        public Guid? HistoryStatusId { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public bool IsMatched(IStatusDefinitionFilterContext context)
        {
            var openStatusId = new Guid("7eb94106-43f1-43eb-8952-8f0b585fd7e5");
            var closedStatusId = new Guid("f299eb6d-b50c-4338-812f-142d4d8515ca");
            var pendingStatusId = new Guid("05a87955-dc2a-4e22-a879-6bea3c31690e");
            if (HistoryStatusId.HasValue)
            {
                if (context.StatusDefinition.StatusDefinitionId == HistoryStatusId.Value)
                    return true;
            }
            else if (CaseId.HasValue)
            {
                GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
                var caseInfo = genericBusinessEntityManager.GetGenericBusinessEntity(CaseId.Value, BusinessEntityDefinitionId);
                if (caseInfo != null && caseInfo.FieldValues != null)
                {
                    object fieldValues;
                    if (caseInfo.FieldValues.TryGetValue("StatusId", out fieldValues))
                    {
                        Guid oldStatusId = Guid.Parse(fieldValues.ToString());

                        if (oldStatusId == context.StatusDefinition.StatusDefinitionId)
                            return true;
                        if (oldStatusId == openStatusId)
                        {
                            if (context.StatusDefinition.StatusDefinitionId == closedStatusId || context.StatusDefinition.StatusDefinitionId == pendingStatusId)
                                return true;
                        }
                        else if (oldStatusId == pendingStatusId)
                        {
                            if (context.StatusDefinition.StatusDefinitionId == closedStatusId)
                                return true;
                        }

                    }
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
