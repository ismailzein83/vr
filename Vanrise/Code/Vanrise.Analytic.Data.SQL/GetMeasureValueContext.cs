using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data.SQL
{
    public class GetMeasureValueContext : IGetMeasureValueContext
    {
        public object GetColumnValue(int columnIndex)
        {
            throw new NotImplementedException();
        }

        public object GetMeasureValue(string measureConfigName)
        {
            throw new NotImplementedException();
        }
    }
}
