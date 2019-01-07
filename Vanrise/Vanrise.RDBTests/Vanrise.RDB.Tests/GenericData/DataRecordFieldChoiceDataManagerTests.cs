//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.GenericData.Data;
//using Vanrise.GenericData.Entities;
//using Vanrise.RDBTests.Common;

//namespace Vanrise.RDB.Tests.GenericData
//{
//    [TestClass]
//    public class DataRecordFieldChoiceDataManagerTests
//    {
//        const string DBTABLE_NAME_DataRecordFieldChoice = "DataRecordFieldChoice";

//        IDataRecordFieldChoiceDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IDataRecordFieldChoiceDataManager>();
//        IDataRecordFieldChoiceDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IDataRecordFieldChoiceDataManager>();
//        [TestMethod]
//        public void AddUpdateSelectDataRecordFieldChoices()
//        {
//            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_GenericData, DBTABLE_NAME_DataRecordFieldChoice);
//            DataRecordFieldChoice DataRecordFieldChoice = new DataRecordFieldChoice
//            {
//                DataRecordFieldChoiceId = Guid.NewGuid(),
//                Name = "RDB TEST",
//                Settings = new DataRecordFieldChoiceSettings
//               {
//                   Choices = new List<Choice>
//                   {
//                       new Choice
//                       {
//                           Text = "Value",
//                           Value = 1
//                       }
//                   }
//               }
//            };
//            var tester = new DataRecordFieldChoiceMainOperationTester(DataRecordFieldChoice, _rdbDataManager, _sqlDataManager);
//            tester.ExecuteTest(true, true, false);
//        }

//        #region Private Classes

//        private class DataRecordFieldChoiceMainOperationTester : EntityMainOperationTester<DataRecordFieldChoice, IDataRecordFieldChoiceDataManager>
//        {
//            public DataRecordFieldChoiceMainOperationTester(DataRecordFieldChoice DataRecordFieldChoice, IDataRecordFieldChoiceDataManager rdbDataManager, IDataRecordFieldChoiceDataManager sqlDataManager)
//                : base(DataRecordFieldChoice, rdbDataManager, sqlDataManager)
//            {

//            }

//            public override void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<DataRecordFieldChoice> context)
//            {
//                DataRecordFieldChoice entity = context.Entity;
//                entity.Name = entity.Name + " Updated ";
//            }
//            public override IEnumerable<DataRecordFieldChoice> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<IDataRecordFieldChoiceDataManager> context)
//            {
//                return context.DataManager.GetDataRecordFieldChoices();
//            }

//            public override bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<DataRecordFieldChoice, IDataRecordFieldChoiceDataManager> context)
//            {
//                return context.DataManager.AddDataRecordFieldChoice(context.Entity);
//            }

//            public override bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<DataRecordFieldChoice, IDataRecordFieldChoiceDataManager> context)
//            {
//                return context.DataManager.UpdateDataRecordFieldChoice(context.Entity);
//            }

//            public override string DBConnStringName => Constants.CONNSTRING_NAME_CONFIG;

//            public override string DBSchemaName => Constants.DBSCHEMA_NAME_GenericData;

//            public override string DBTableName => DBTABLE_NAME_DataRecordFieldChoice;

//            public override void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<DataRecordFieldChoice> context)
//            {
//                context.Entity.Name = Guid.NewGuid().ToString();
//            }

//            public override void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<DataRecordFieldChoice> context)
//            {
//                context.TargetEntity.Name = context.SourceEntity.Name;
//            }

//            public override void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<DataRecordFieldChoice, IDataRecordFieldChoiceDataManager> context)
//            {
//                throw new NotImplementedException();
//            }

//            public override void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<DataRecordFieldChoice> context)
//            {
//                context.Entity.DataRecordFieldChoiceId = Guid.NewGuid();
//            }
//        }

//        #endregion
//    }
//}
