using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public enum NumberType { PhoneNumber = 1, DP = 2, FDB = 3 }

    public enum ConnectionType { IMSPhoneNumberTID = 1, IMSOLT = 2, OLTSplitterLogical = 3, SplitterFDB = 4 }
}