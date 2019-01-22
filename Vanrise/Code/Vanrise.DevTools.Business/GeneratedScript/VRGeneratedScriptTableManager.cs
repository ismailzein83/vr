using Vanrise.DevTools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.DevTools.Entities;
using Vanrise.Common.Business;
namespace Vanrise.DevTools.Business
{
    public class VRGeneratedScriptTableManager
    {

        #region Public Methods


        public IEnumerable<VRGeneratedScriptTableInfo> GetTablesInfo(VRGeneratedScriptTableInfoFilter tableInfoFilter)
        {
            IVRGeneratedScriptTableDataManager tableDataManager = VRDevToolsFactory.GetDataManager<IVRGeneratedScriptTableDataManager>();


            Guid connectionId = tableInfoFilter.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;

            tableDataManager.Connection_String = connectionString;

            List<VRGeneratedScriptTable> allTables = tableDataManager.GetTables(tableInfoFilter.SchemaName);

            return allTables.MapRecords(TableInfoMapper).OrderBy(table => table.Name);
        }


        #endregion

        #region Private Classes

        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        public VRGeneratedScriptTableInfo TableInfoMapper(VRGeneratedScriptTable table)
        {
            return new VRGeneratedScriptTableInfo
            {
                Name = table.Name,
            };
        }
        #endregion

    }
}
