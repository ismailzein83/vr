using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class InsertDealOperationOutput<T> : Vanrise.Entities.InsertOperationOutput<T>
    {
        public List<string> ValidationMessages { get; set; }
    }
}
