using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public interface IRulePhoneNumberPrefixCriteria
    {
        IEnumerable<string> PhoneNumberPrefixes { get; }
    }
}
