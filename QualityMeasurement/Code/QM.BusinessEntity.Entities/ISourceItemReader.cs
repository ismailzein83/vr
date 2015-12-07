using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public interface ISourceItemReader<T> where T : ISourceItem
    {
        bool UseSourceItemId { get; }

        IEnumerable<T> GetChangedItems(ref object updatedHandle);
    }
}
