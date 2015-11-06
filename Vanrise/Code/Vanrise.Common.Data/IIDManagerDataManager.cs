using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IIDManagerDataManager : IDataManager
    {
        void ReserveIDRange(int typeId, int nbOfIds, out long startingId);
    }
}
