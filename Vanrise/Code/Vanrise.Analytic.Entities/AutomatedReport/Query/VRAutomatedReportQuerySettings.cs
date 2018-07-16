using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class VRAutomatedReportQuerySettings
    {
        public abstract VRAutomatedReportDataResult Execute(IVRAutomatedReportQueryExecuteContext context);

        public abstract VRAutomatedReportDataSchema GetSchema(IVRAutomatedReportQueryGetSchemaContext context);

        public virtual Dictionary<string, VRAutomatedReportDataFieldSchema> GetFields(IVRAutomatedReportQueryGetFieldsContext context)
        {
            return null;
        }
    }

    public interface IVRAutomatedReportQueryExecuteContext
    {
        Guid QueryDefinitionId { get; }
    }

    public interface IVRAutomatedReportQueryGetSchemaContext
    {
        Guid QueryDefinitionId { get; }
    }

    public interface IVRAutomatedReportQueryGetFieldsContext
    {
        Guid QueryDefinitionId { get; }
    }
}
