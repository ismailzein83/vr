using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IWidgetDefinitionDataManager:IDataManager
    {
        List<WidgetDefinition> GetWidgetDefinitions();
        bool AreWidgetDefinitionUpdated(ref object updateHandle);
    }
}
