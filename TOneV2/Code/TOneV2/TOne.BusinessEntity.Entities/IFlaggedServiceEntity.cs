using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public interface IFlaggedServiceEntity
    {
        short FlaggedServiceID { get; set; }
        string FlaggedServiceSymbol { get; set; }
        string FlaggedServiceColor { get; set; }
    }
}
