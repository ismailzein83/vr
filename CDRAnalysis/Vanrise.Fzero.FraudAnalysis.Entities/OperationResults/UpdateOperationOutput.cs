using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum UpdateOperationResult { Succeeded = 0, Failed = 1}
    
    public class UpdateOperationOutput<T>
    {
        public UpdateOperationResult Result { get; set; }

        public T UpdatedObject { get; set; }
    }
}
