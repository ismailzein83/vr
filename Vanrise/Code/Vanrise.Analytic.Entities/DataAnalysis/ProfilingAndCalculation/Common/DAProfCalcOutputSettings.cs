using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class DAProfCalcOutputSettings : DataAnalysisItemDefinitionSettings
    {
        public Guid RecordTypeId { get; set; }

        public abstract List<DataRecordField> GetOutputFields(IDAProfCalcOutputSettingsGetOutputFieldsContext context);
    }

    public interface IDAProfCalcOutputSettingsGetOutputFieldsContext
    {

    }
}
