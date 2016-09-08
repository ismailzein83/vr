using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class DefaultServicePreviewDataManager : BaseSQLDataManager, IDefaultServicePreviewDataManager
    {
        #region Fields / Properties

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }

        #endregion

        #region Constructors

        public DefaultServicePreviewDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public bool Insert(DefaultServicePreview preview)
        {
            string currentServices = preview.CurrentServices != null ? Vanrise.Common.Serializer.Serialize(preview.CurrentServices) : null;
            string newServices = preview.NewServices != null ? Vanrise.Common.Serializer.Serialize(preview.NewServices) : null;

            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_DefaultServicePreview_Insert", _processInstanceId, currentServices, preview.IsCurrentServiceInherited, newServices, preview.EffectiveOn, preview.EffectiveUntil);

            return affectedRows > 0;
        }

        public DefaultServicePreview Get(RatePlanPreviewQuery query)
        {
            return GetItemSP("TOneWhS_Sales.sp_DefaultServicePreview_Get", DefaultServicePreviewMapper, query.ProcessInstanceId);
        }

        #endregion

        #region Mappers

        private DefaultServicePreview DefaultServicePreviewMapper(IDataReader reader)
        {
            var preview = new DefaultServicePreview()
            {
                IsCurrentServiceInherited = GetReaderValue<bool?>(reader, "IsCurrentServiceInherited"),
                EffectiveOn = (DateTime)reader["EffectiveOn"],
                EffectiveUntil = GetReaderValue<DateTime?>(reader, "EffectiveUntil")
            };

            string currentServices = reader["CurrentServices"] as string;
            if (currentServices != null)
                preview.CurrentServices = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(currentServices);

            string newServices = reader["NewServices"] as string;
            if (newServices != null)
                preview.NewServices = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(newServices);

            return preview;
        }

        #endregion
    }
}
