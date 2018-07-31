using System;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class SwitchDBReplicationPreInsert : DBReplicationPreInsert
    {
        public override Guid ConfigId { get { return new Guid("5f22d483-6913-4943-bbbb-804559bef664"); } }
        public override void Execute(IDBReplicationPreInsertExecuteContext context)
        {
            if (context.DataToInsert == null || context.DataToInsert.Rows == null)
                return;

            DataRowCollection rows = context.DataToInsert.Rows;

            foreach (DataRow row in rows)
            {
                string serializedSwitchSettings = row["Settings"] as string;
                if (string.IsNullOrEmpty(serializedSwitchSettings))
                    continue;

                SwitchSettings switchSettings = Vanrise.Common.Serializer.Deserialize<SwitchSettings>(serializedSwitchSettings);
                if (switchSettings.RouteSynchronizer == null)
                    continue;

                switchSettings.RouteSynchronizer.RemoveConnection(new SwitchRouteSynchronizerRemoveConnectionContext());

                string switchSettingsString = Vanrise.Common.Serializer.Serialize(switchSettings);
                row["Settings"] = switchSettingsString;
            }
        }
    }
}