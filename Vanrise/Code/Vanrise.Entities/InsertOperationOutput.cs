using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum InsertOperationResult { Succeeded = 0, Failed = 1, SameExists = 2 }
    public class InsertOperationOutput<T>
    {
        public InsertOperationResult Result { get; set; }

        public T InsertedObject { get; set; }

        public string Message { get; set; }

        public bool ShowExactMessage { get; set; }
        public List<InsertAdditionalMessage> AdditionalMessages { get; set; }
    }
    public class InsertAdditionalMessage
    {
        public InsertOperationResult Result { get; set; }
        public string Message { get; set; }
    }
}
