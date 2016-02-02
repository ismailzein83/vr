using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation
{
    public class DataTransformationRuntimeType
    {
        public Type ExecutorType { get; set; }
    }

    public interface IDataTransformationExecutor
    {
        void Execute();
    }
}
