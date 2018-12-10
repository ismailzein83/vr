//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.BusinessProcess.Entities;
//using Vanrise.Data.RDB;

//namespace Vanrise.BusinessProcess.Data.RDB
//{
//    public class BPBusinessRuleDefinitionDataManager : IBPBusinessRuleDefinitionDataManager
//    {
//        #region Constructor

//        static string TABLE_NAME = "bp_BPBusinessRuleDefinition";
//        const string COL_ID = "ID";
//        const string COL_Name = "Name";
//        const string COL_BPDefintionId = "BPDefintionId";
//        const string COL_Settings = "Settings";
//        const string COL_Rank = "Rank";


//        static BPBusinessRuleDefinitionDataManager()
//        {
//            var columns = new Dictionary<string, RDBTableColumnDefinition>();
//            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
//            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
//            columns.Add(COL_BPDefintionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
//            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
//            columns.Add(COL_Rank, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
//            {
//                DBSchemaName = "bp",
//                DBTableName = "BPBusinessRuleDefinition",
//                Columns = columns,
//                IdColumnName = COL_ID
//            });
//        }
//        #endregion

//        #region Public Methods
//        public bool AreBPBusinessRuleDefinitionsUpdated(ref object updateHandle)
//        {
//            throw new NotImplementedException();
//        }

//        public List<BPBusinessRuleDefinition> GetBPBusinessRuleDefinitions()
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//        #region Mappers

//        #endregion
//    }
//}
