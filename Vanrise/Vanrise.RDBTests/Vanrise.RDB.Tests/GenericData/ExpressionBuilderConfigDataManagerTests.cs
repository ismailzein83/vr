using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.RDBTests.Common;
using Vanrise.GenericData.Entities.ExpressionBuilder;
namespace Vanrise.RDB.Tests.GenericData
{
    [TestClass]
    public class ExpressionBuilderConfigDataManagerTests
    {
        const string DBTABLE_NAME_ExpressionBuilderConfig = "ExpressionBuilderConfig";

        IExpressionBuilderConfigDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IExpressionBuilderConfigDataManager>();
        IExpressionBuilderConfigDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IExpressionBuilderConfigDataManager>();
        [TestMethod]
        public void AddUpdateSelectExpressionBuilderConfig()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_ExpressionBuilderConfig);
            ExpressionBuilderConfig ExpressionBuilderConfig = new ExpressionBuilderConfig
            {
                ExpressionBuilderConfigId = 1,
                Name = "RDB TEST",
                Editor = "RDBTests",
                Title = "RDBTests",
            };
            var rdbResponse = _rdbDataManager.GetExpressionBuilderTemplates();
            var sqlResponse = _sqlDataManager.GetExpressionBuilderTemplates();
            UTUtilities.AssertObjectsAreSimilar(rdbResponse, sqlResponse);
        }
    }
}
