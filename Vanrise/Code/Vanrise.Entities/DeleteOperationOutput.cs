using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum DeleteOperationResult { Succeeded = 0, Failed = 1 }
    public class DeleteOperationOutput<T>
    {
        public DeleteOperationResult Result { get; set; }
        public string Message { get; set; }
    }
}
