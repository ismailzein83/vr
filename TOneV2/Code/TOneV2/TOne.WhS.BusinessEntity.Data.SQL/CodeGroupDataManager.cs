using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CodeGroupDataManager : BaseSQLDataManager, ICodeGroupDataManager
    {

        #region ctor/Local Variables
        public CodeGroupDataManager(): base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        readonly string[] columns = {"CountryID", "Code"};

        #endregion

        #region Public Methods
        public List<CodeGroup> GetCodeGroups()
        {
            return GetItemsSP("TOneWhS_BE.sp_CodeGroup_GetAll", CodeGroupMapper);
        }
        public bool Update(CodeGroup codeGroup)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CodeGroup_Update", codeGroup.CodeGroupId, codeGroup.CountryId, codeGroup.Code);
            return (recordsEffected > 0);
        }
        public bool Insert(CodeGroup codeGroup, out int insertedId)
        {
            object codeGroupId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CodeGroup_Insert", out codeGroupId, codeGroup.Code, codeGroup.CountryId);
            insertedId = (int)codeGroupId;
            return (recordsEffected > 0);
        }
        public bool AreCodeGroupUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CodeGroup", ref updateHandle);
        }
        public void SaveCodeGroupToDB(List<CodeGroup> codeGroups)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (CodeGroup codeGroup in codeGroups)
                WriteRecordToStream(codeGroup, dbApplyStream);
            Object preparedCodeGroups = FinishDBApplyStream(dbApplyStream);
            ApplyCodeGroupsToDB(preparedCodeGroups);
        }
        #endregion
       
        #region Private Methods
        private CodeGroup CodeGroupMapper(IDataReader reader)
        {
            CodeGroup codeGroup = new CodeGroup
            {
                CodeGroupId = (int)reader["ID"],
                Code = reader["Code"] as string,
                CountryId = (int)reader["CountryID"]

            };

            return codeGroup;
        }
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private void WriteRecordToStream(CodeGroup record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}",
                                     record.CountryId,
                                     record.Code);

        }
        private void ApplyCodeGroupsToDB(Object preparedCodeGroups)
        {
            InsertBulkToTable(preparedCodeGroups as StreamBulkInsertInfo);
        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = columns,
                TableName = "[TOneWhS_BE].[CodeGroup]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        #endregion

    }
}
