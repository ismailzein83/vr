using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public enum RDBDataType
    {
        [Description("Varchar")]
        Varchar = 0,
        [Description("NVarchar")]
        NVarchar = 1,
        [Description("Int")]
        Int = 2,
        [Description("BigInt")]
        BigInt = 3,
        [Description("Decimal")]
        Decimal = 4,
        [Description("DateTime")]
        DateTime = 5,
        [Description("UniqueIdentifier")]
        UniqueIdentifier = 6,
        [Description("Boolean")]
        Boolean = 7,
        [Description("VarBinary")]
        VarBinary = 8,
        [Description("Cursor")]
        Cursor = 9
    }
}
