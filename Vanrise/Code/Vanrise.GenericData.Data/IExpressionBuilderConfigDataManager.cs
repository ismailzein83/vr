using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities.ExpressionBuilder;

namespace Vanrise.GenericData.Data
{
    public interface IExpressionBuilderConfigDataManager:IDataManager
    {
        List<ExpressionBuilderConfig> GetExpressionBuilderTemplates();
        bool AreExpressionBuilderConfigUpdated(ref object updateHandle);
    }
}
