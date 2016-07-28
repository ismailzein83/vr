using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class DefaultRoutingProductPreviewDataManager : BaseSQLDataManager, IDefaultRoutingProductPreviewDataManager
    {
        #region Fields / Properties

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }
        
        #endregion

        #region Constructors

        public DefaultRoutingProductPreviewDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public bool Insert(DefaultRoutingProductPreview preview)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_DefaultRoutingProductPreview_Insert", _processInstanceId, preview.CurrentDefaultRoutingProductName, preview.IsCurrentDefaultRoutingProductInherited, preview.NewDefaultRoutingProductName, preview.EffectiveOn);

            return affectedRows > 0;
        }

        public DefaultRoutingProductPreview Get(RatePlanPreviewQuery query)
        {
            return GetItemSP("TOneWhS_Sales.sp_DefaultRoutingProductPreview_Get", DefaultRoutingProductPreviewMapper, query.ProcessInstanceId);
        }

        #endregion

        #region Mappers

        private DefaultRoutingProductPreview DefaultRoutingProductPreviewMapper(IDataReader reader)
        {
            return new DefaultRoutingProductPreview()
            {
                CurrentDefaultRoutingProductName = reader["CurrentDefaultRoutingProductName"] as string,
                IsCurrentDefaultRoutingProductInherited = GetReaderValue<bool?>(reader, "IsCurrentDefaultRoutingProductInherited"),
                NewDefaultRoutingProductName = reader["NewDefaultRoutingProductName"] as string,
                EffectiveOn = (DateTime)reader["EffectiveOn"]
            };
        }
        
        #endregion
    }
}
