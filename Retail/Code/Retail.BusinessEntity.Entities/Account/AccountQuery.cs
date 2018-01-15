using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountQuery
    {
        public Guid AccountBEDefinitionId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Guid> AccountTypeIds { get; set; }
        public bool OnlyRootAccount { get; set; }
        public List<Guid> StatusIds { get; set; }
        public long? ParentAccountId { get; set; } 
        public List<string> Columns { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }
    }
}
