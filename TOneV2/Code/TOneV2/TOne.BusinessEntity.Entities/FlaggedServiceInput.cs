using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class FlaggedServiceInput : IFlaggedServiceEntity
    {
        public short FlaggedServiceID
        {
            get;
            set;
        }

        public string FlaggedServiceSymbol
        {
            get;
            set;
        }

        public string FlaggedServiceColor
        {
            get;
            set;
        }
    }
}
