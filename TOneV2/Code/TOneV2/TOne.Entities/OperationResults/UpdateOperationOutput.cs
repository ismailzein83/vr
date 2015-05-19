using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public enum UpdateOperationResult { Succeeded = 0, Failed = 1}
    
    public class UpdateOperationOutput
    {
        public UpdateOperationResult Result { get; set; }
    }
}
