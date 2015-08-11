using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IRepeatedNumbersDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<RepeatedNumbers> GetRepeatedNumbersData(Vanrise.Entities.DataRetrievalInput<RepeatedNumbersInput> input);
        
    }
}
