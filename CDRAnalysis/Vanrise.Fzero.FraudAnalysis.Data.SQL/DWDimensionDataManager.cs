using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DWDimensionDataManager : BaseSQLDataManager, IDWDimensionDataManager
    {

        public DWDimensionDataManager()
            : base("DWSDBConnString")
        {

        }

        public List<Dimension> GetDimensions(string tableName)
        {
            string query = string.Format("select * from {0}", tableName);
            return GetItemsText(query, DimensionMapper, (cmd) => { });
        }


        #region Private Methods

        private Dimension DimensionMapper(IDataReader reader)
        {
            var dimension = new Dimension();
            dimension.Id = (int)reader[0];
            dimension.Description = reader[1] as string;
            return dimension;
        }

        #endregion



    }
}
