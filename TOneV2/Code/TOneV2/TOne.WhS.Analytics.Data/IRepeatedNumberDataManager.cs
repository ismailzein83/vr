using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Data
{
    public interface IRepeatedNumberDataManager : IDataManager
    {
        IEnumerable<RepeatedNumber> GetRepeatedNumberData(Vanrise.Entities.DataRetrievalInput<RepeatedNumberInput> input);
    }
}
