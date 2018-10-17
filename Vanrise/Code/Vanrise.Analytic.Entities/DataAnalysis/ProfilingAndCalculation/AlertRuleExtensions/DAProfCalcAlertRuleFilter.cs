using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class DAProfCalcAlertRuleFilter
    {
        public abstract Guid ConfigId { get; }

        public abstract RecordFilterGroup GetRecordFilterGroup(IDAProfCalcGetRecordFilterGroupContext context);
    }

    public interface IDAProfCalcGetRecordFilterGroupContext
    {
    }

    public class DAProfCalcGetRecordFilterGroupContext : IDAProfCalcGetRecordFilterGroupContext
    {
    }
}