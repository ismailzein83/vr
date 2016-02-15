using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Entities;

namespace Vanrise.BI.Data.SQL
{
    public class DimensionDataManager : BaseDataManager, IDimensionDataManager
    { 
        #region ctor

        #endregion
        public List<DimensionInfo> GetDimensionInfo(string entityName)
        {
            string dimensionId;
            string dimensionName;
            GetEntityColumn(entityName,out dimensionId ,out dimensionName);
            StringBuilder builderQuery = new StringBuilder();

            builderQuery.Append(String.Format(@"with member Measures.[DataItems] as '1' select Measures.[DataItems] on columns, NonEmptyCrossjoin({0}.children ,
		    Descendants({1}.currentmember,{1})) on rows
	        from [{2}]",dimensionId, dimensionName, CubeName));

            List<DimensionInfo> dimensionInfoResult = new List<DimensionInfo>();

            ExecuteReaderMDX(builderQuery.ToString(), (reader) =>
            {
                while (reader.Read())
                {
                    DimensionInfo dimensionInfo = new DimensionInfo
                    {
                        DimensionId = reader[GetRowColumnToRead(dimensionId)],
                        Name = reader[GetRowColumnToRead(dimensionName)] as string
                    };
                    dimensionInfoResult.Add(dimensionInfo);
                }
            });

            return dimensionInfoResult;
        }
        void GetEntityColumn(string entityName, out string dimensionId, out string dimensionName)
        {
                dimensionId = null;
                dimensionName = null;
                foreach (BIConfiguration<BIConfigurationEntity> obj in _entityDefinitions)
                {

                    if (entityName == obj.Name)
                    {
                        dimensionId = obj.Configuration.ColumnID;
                        dimensionName = obj.Configuration.ColumnName;
                    }
                }
        }
    }
}
