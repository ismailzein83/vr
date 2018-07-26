using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.DBReplication
{
    public class DBReplicationPreInsertConfig: ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VRCommon_DBReplicationPreInsert";

        public string Editor { get; set; }
    }
}
