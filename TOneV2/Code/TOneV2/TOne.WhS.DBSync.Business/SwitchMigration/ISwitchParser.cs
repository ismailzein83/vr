using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public interface ISwitchParser
    {
        void GetParsedMappings(out List<InParsedMapping> inParsedMappings, out  List<OutParsedMapping> outParsedMappings);

    }
}
