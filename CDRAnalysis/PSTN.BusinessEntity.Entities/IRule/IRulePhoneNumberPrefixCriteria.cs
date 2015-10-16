using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public interface IRulePhoneNumberPrefixCriteria
    {
        IEnumerable<string> PhoneNumberPrefixes { get; }
    }
}
