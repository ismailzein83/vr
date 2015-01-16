using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;

namespace TOne.Data.SQL
{
    public class LCRDataManager : BaseTOneDataManager, ILCRDataManager
    {
        public void LoadUpdatedCodes(byte[] timestamp, Action<List<LCRCode>> onCodeGroupListReady)
        {
            ExecuteReaderCmdText(GET_UPDATEDCodes, (reader) =>
                {
                    List<LCRCode> codeGroupList = new List<LCRCode>();
                    string currentCodeGroup = null;
                    while (reader.Read())
                    {
                        LCRCode code = new LCRCode
                        {
                            ID = (long)reader["ID"],
                            Value = reader["Code"] as string,
                            BeginEffectiveDate = reader["BeginEffectiveDate"] as Nullable<DateTime>,
                            EndEffectiveDate = GetReaderValue<Nullable<DateTime>>(reader, "EndEffectiveDate"),
                            CodeGroup = reader["CodeGroup"] as string
                        };
                        if (code.CodeGroup != currentCodeGroup)
                        {
                            if (codeGroupList.Count > 0)
                                onCodeGroupListReady(codeGroupList);
                            codeGroupList.Clear();
                            currentCodeGroup = code.CodeGroup;
                        }
                    }
                    if (codeGroupList.Count > 0)
                        onCodeGroupListReady(codeGroupList);
                },
                null);
        }

        #region Queries

        const string GET_UPDATEDCodes = @"select c.[ID]
      ,c.[Code]
      ,c.[ZoneID]
      ,c.[BeginEffectiveDate]
      ,c.[EndEffectiveDate]
      ,c.[IsEffective]
      ,c.[UserID]
      ,c.[timestamp]
      ,z.CodeGroup from Code c With(NoLock)
JOIN Zone z With(NoLock) ON c.ZoneID = z.ZoneID
 order by z.CodeGroup";

        #endregion

        #region ILCRDataManager Members


        public List<string> GetUpdatedCodeGroups(byte[] p)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}