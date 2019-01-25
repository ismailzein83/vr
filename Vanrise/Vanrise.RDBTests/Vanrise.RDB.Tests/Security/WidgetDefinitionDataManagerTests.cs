using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.RDBTests.Common;
using Vanrise.Security.Data;

namespace Vanrise.RDB.Tests.Security
{
    [TestClass]
    public class WidgetDefinitionDataManagerTests
    {
        const string DBTABLE_NAME_RequiredPermission = "WidgetDefinition";

        IWidgetDefinitionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IWidgetDefinitionDataManager>();
        IWidgetDefinitionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IWidgetDefinitionDataManager>();
        [TestMethod]
        public void GetWidgetDefinitions()
        {
            var rdbResponse = _rdbDataManager.GetWidgetsDefinition();
            var sqlResponse = _sqlDataManager.GetWidgetsDefinition();
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
        }
    }
}
