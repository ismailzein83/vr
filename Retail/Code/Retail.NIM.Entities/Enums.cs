using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public enum NumberType { PhoneNumber = 1, DP = 2, FDB = 3 }

    public enum ConnectionType { IMSOLT = 1, OLTHorizontalVertical = 2, OLTSplitter = 3, SplitterInOut = 4, SplitterFDB = 5 }
}