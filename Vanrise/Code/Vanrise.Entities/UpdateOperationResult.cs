using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum UpdateOperationResult { Succeeded = 0, Failed = 1, SameExists = 2 }

    public class UpdateOperationOutput<T>
    {
        public UpdateOperationResult Result { get; set; }
        public string Message { get; set; }
        public bool ShowExactMessage { get; set; }
        public T UpdatedObject { get; set; }
        public List<UpdateAdditionalMessage> AdditionalMessages { get; set; }
    }
    public class UpdateAdditionalMessage
    {
        public UpdateOperationResult Result { get; set; }
        public string Message { get; set; }
    }
}
