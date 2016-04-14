using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IMeasureEvaluator
    {
        List<string> GetColumns(IGetMeasureColumnsContext context);

        Object GetMeasureValue(IGetMeasureValueContext context);
    }

    public interface IGetMeasureColumnsContext
    {

    }

    public interface IGetMeasureValueContext
    {
        Object GetColumnValue(int columnIndex);

        Object GetMeasureValue(string measureConfigName);
    }

}
