using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.GenericData.RDBDataStorage.MainExtensions.Joins
{
    public class RDBDataRecordStorageJoinOtherRecordStorage : RDBDataRecordStorageJoinSettings
    {
        public override Guid ConfigId => new Guid("E5FA86E6-FA20-4301-9FFE-E20A57BEC459");

        public Guid RecordStorageId { get; set; }

        public RDBJoinType JoinType { get; set; }

        public List<RDBDataRecordStorageJoinOtherRecordStorageCondition> JoinConditions { get; set; }

        public override string GetCode(IRDBDataRecordStorageJoinExpressionGetCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("var tableToJoinTableAlias = context.GetTableToJoinTableAlias();");

            codeBuilder.AppendLine($@"var otherDataBaseName = Vanrise.GenericData.Business.Helper.GetStorageName(new Guid(""{ this.RecordStorageId }""));");
            codeBuilder.AppendLine("var databaseName = context.RDBJoinContext.QueryBuilderContext.DataProvider.GetDataBaseName(new Vanrise.Data.RDB.RDBDataProviderGetDataBaseNameContext());");

            codeBuilder.AppendLine("Vanrise.Data.RDB.RDBJoinStatementContext joinStatement;");

            codeBuilder.AppendLine("if (string.Compare(databaseName, otherDataBaseName, true) != 0)");
            codeBuilder.AppendLine($@"joinStatement = context.RDBJoinContext.Join(otherDataBaseName, RecordStorageRDBAnalyticDataProviderTable.GetRDBTableNameByRecordStorageId(new Guid(""{ this.RecordStorageId }"")), tableToJoinTableAlias);");
            codeBuilder.AppendLine("else");
            codeBuilder.AppendLine($@"joinStatement = context.RDBJoinContext.Join(RecordStorageRDBAnalyticDataProviderTable.GetRDBTableNameByRecordStorageId(new Guid(""{this.RecordStorageId}"")), tableToJoinTableAlias);");

            codeBuilder.AppendLine($"joinStatement.JoinType(Vanrise.Data.RDB.RDBJoinType.{this.JoinType.ToString()});");

            codeBuilder.AppendLine("var onCondition = joinStatement.On();");
            foreach (var condition in this.JoinConditions)
            {
                if (!String.IsNullOrEmpty(condition.SourceStorageJoinName))
                    codeBuilder.AppendLine($@"string storageToCompareTableAlias = context.GetJoinTableAlias(""{condition.SourceStorageJoinName}"");");
                else
                    codeBuilder.AppendLine($"string storageToCompareTableAlias = context.MainTableAlias;");
                codeBuilder.AppendLine($@"onCondition.EqualsCondition(tableToJoinTableAlias, ""{condition.StorageToJoinFieldName}"").Column(storageToCompareTableAlias, ""{condition.SourceStorageFieldName}"");");
            }

            return codeBuilder.ToString();
        }

        public override List<string> GetDependentJoins(IRDBDataRecordStorageJoinExpressionGetDependentJoinsContext context)
        {
            return this.JoinConditions.Where(itm => !string.IsNullOrEmpty(itm.SourceStorageJoinName)).Select(itm => itm.SourceStorageJoinName).Distinct().ToList();
        }

        public override string StorageFieldEditor
        {
            get
            {
                return "vr-genericadata-rdbdatarecordstoragesettings-joinotherrecordstorage-storagefieldeditor";
            }
        }
    }

    public class RDBDataRecordStorageJoinOtherRecordStorageCondition
    {

        /// <summary>
        /// this is optional, it will be the current storage in case it is null
        /// </summary>
        public string SourceStorageJoinName { get; set; }

        public string SourceStorageFieldName { get; set; }

        public string StorageToJoinFieldName { get; set; }
    }
}
