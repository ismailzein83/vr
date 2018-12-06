
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Data.RDB;
//using Vanrise.Reprocess.Data;
//using Vanrise.Reprocess.Entities;

//namespace Vanrise.Reprocess.Data.RDB
//{
//    public class ReprocessDefinitionDataManager : IReprocessDefinitionDataManager
//    {
//        #region Constructor

//        static string TABLE_NAME = "reprocess_ReprocessDefinition";
//        const string COL_Id = "Id";
//        const string COL_Name = "Name";
//        const string COL_Settings = "Settings";
//        const string COL_CreatedTime = "CreatedTime";


//        static ReprocessDefinitionDataManager()
//        {
//            var columns = new Dictionary<string, RDBTableColumnDefinition>();
//            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
//            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
//            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
//            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
//            {
//                DBSchemaName = "reprocess",
//                DBTableName = "ReprocessDefinition",
//                Columns = columns,
//                IdColumnName = COL_Id,
//                CreatedTimeColumnName = COL_CreatedTime
//            });
//        }
//        #endregion

//        public List<ReprocessDefinition> GetReprocessDefinition()
//        {
//            throw new NotImplementedException();
//        }

//        public bool Insert(ReprocessDefinition ReprocessDefinitionItem)
//        {
//            throw new NotImplementedException();
//        }

//        public bool Update(ReprocessDefinition ReprocessDefinitionItem)
//        {
//            throw new NotImplementedException();
//        }
//    }

//}
