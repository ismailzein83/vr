using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericBEOnBeforeInsertHandlerContext : IGenericBEOnBeforeInsertHandlerContext
    {
        public GenericBusinessEntity GenericBusinessEntity { get; set; }
        public GenericBusinessEntity OldGenericBusinessEntity { get; set; }
        public GenericBEDefinitionSettings DefinitionSettings { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public HandlerOperationType OperationType { get; set; }
        public OutputResult OutputResult { get; set; }

    }
    public class OutputResult
    {
        public bool Result { get; set; }
        public List<string> Messages { get; set; }
    }
}
