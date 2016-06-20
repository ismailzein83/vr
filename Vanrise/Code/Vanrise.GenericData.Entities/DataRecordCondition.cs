using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataRecordCondition
    {
        public abstract bool Evaluate(IDataRecordConditionContext context);
    }

    public interface IDataRecordConditionContext
    {
        dynamic DataRecord { get; }
    }
}
