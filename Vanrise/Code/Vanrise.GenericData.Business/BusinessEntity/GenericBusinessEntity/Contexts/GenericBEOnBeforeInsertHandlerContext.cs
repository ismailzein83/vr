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

        public GenericBEDefinitionSettings DefinitionSettings { get; set; }

        public Guid BusinessEntityDefinitionId { get; set; }
    }
}
