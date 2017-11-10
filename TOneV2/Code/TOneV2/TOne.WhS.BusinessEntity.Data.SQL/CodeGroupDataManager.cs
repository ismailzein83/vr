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
        public CodeGroupDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        readonly string[] columns = { "CountryID", "Code", "Name" };

        #endregion

        #region Public Methods
        public List<CodeGroup> GetCodeGroups()
        {
            return GetItemsSP("TOneWhS_BE.sp_CodeGroup_GetAll", CodeGroupMapper);
        }

        public bool CheckIfCodeGroupHasRelatedCodes(int codeGroupId)
        {
            return (Convert.ToBoolean(ExecuteScalarSP("TOneWhS_BE.sp_CheckIfCodeGroupHasRelatedCodes", codeGroupId)));
        }
        public bool Update(CodeGroupToEdit codeGroup)
        {
            codeGroup.Code = codeGroup.Code.Trim();
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CodeGroup_Update", codeGroup.CodeGroupId, codeGroup.CountryId, codeGroup.Code, codeGroup.Name);
            return (recordsEffected > 0);
        }
        public bool Insert(CodeGroup codeGroup, out int insertedId)
        {
            object codeGroupId;
            codeGroup.Code = codeGroup.Code.Trim();
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CodeGroup_Insert", out codeGroupId, codeGroup.Code, codeGroup.CountryId, codeGroup.Name);
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
            {
                codeGroup.Code = codeGroup.Code.Trim();
                WriteRecordToStream(codeGroup, dbApplyStream);
            }
            Object preparedCodeGroups = FinishDBApplyStream(dbApplyStream);
            ApplyCodeGroupsToDB(preparedCodeGroups);
        }
        #endregion

        #region Private Methods

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private void WriteRecordToStream(CodeGroup record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}",
                                     record.CountryId,
                                     record.Code,
                                     record.Name);

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

        #region  Mappers
        private CodeGroup CodeGroupMapper(IDataReader reader)
        {
            CodeGroup codeGroup = new CodeGroup
            {
                CodeGroupId = (int)reader["ID"],
                Code = GetReaderValue<string>(reader, "Code").Trim(),
                CountryId = (int)reader["CountryID"],
                Name = reader["Name"] as string
            };

            return codeGroup;
        }
        #endregion
    }
}
