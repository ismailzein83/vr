using System.Collections.Generic;

namespace InterConnect.BusinessEntity.Entities
{
    public class OperatorAccountQuery
    {
        public string Suffix { get; set; }
        public List<int> OperatorProfileIds { get; set; }
    }
}
