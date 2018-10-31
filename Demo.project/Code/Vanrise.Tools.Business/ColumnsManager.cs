﻿using Vanrise.Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Vanrise.Common;
using Vanrise.Tools.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace Vanrise.Tools.Business
{
    public class ColumnsManager
    {


        #region Public Methods
        public IEnumerable<ColumnsInfo> GetColumnsInfo(ColumnsInfoFilter columnInfoFilter)
        {
            IColumnsDataManager columnsDataManager = VRToolsFactory.GetDataManager<IColumnsDataManager>();

            Guid ConnectionId = columnInfoFilter.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(ConnectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;

            columnsDataManager.Connection_String = connectionString;

            List<Columns> allColumns = columnsDataManager.GetColumns(columnInfoFilter.TableName);

            Func<Columns, bool> filterFunc = (columns) =>
            {
                return true;
            };
            return allColumns.MapRecords(ColumnsInfoMapper, filterFunc).OrderBy(columns => columns.Name);
        }


        #endregion

        #region Private Classes

        #endregion

        #region Private Methods

        #endregion

        #region Mappers

        public ColumnsInfo ColumnsInfoMapper(Columns columns)
        {
            return new ColumnsInfo
            {
                Name = columns.Name,
            };
        }
        #endregion

    }
}
