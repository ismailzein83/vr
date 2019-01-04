using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.RDBTests.Common;
namespace Vanrise.RDB.Tests.Common
{
    [TestClass]
    public class ExtensionConfigurationDataManagerTests
    {
        const string DBTABLE_NAME_Rule = "[ExtensionConfiguration]";
        IExtensionConfigurationDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IExtensionConfigurationDataManager>();
        IExtensionConfigurationDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IExtensionConfigurationDataManager>();
        [TestMethod]
        public void GetExtensionConfigurationsByType()
        {
            var ruleChanged1 = _sqlDataManager.GetExtensionConfigurationsByType<TextManipulationActionSettingsConfig>(TextManipulationActionSettingsConfig.EXTENSION_TYPE);
            var ruleChanged2 = _rdbDataManager.GetExtensionConfigurationsByType<TextManipulationActionSettingsConfig>(TextManipulationActionSettingsConfig.EXTENSION_TYPE);
            UTUtilities.AssertObjectsAreSimilar(ruleChanged1, ruleChanged2);
        }
    }
}
