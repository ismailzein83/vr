using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IUtilityDataManager : IDataManager
    {
        DateTimeRange GetDateTimeRange();

        bool CheckIfDefaultOrInvalid(DateTime? dateTime);
    }
}
