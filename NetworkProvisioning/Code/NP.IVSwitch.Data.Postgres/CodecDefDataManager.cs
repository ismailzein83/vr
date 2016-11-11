using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    class CodecDefDataManager : BasePostgresDataManager, ICodecDefDataManager
    {
        public CodecDefDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private CodecDef CodecDefMapper(IDataReader reader)
        {
            CodecDef CodecDef = new CodecDef
            {
                CodecId = (int)reader["codec_id"],
                FsName = reader["fs_name"] as string,
                Description = reader["description"] as string,
                ClockRate = (int)reader["clock_rate"],
                DefaultMsPerPacket = (int)reader["default_ms_per_packet"],
                PassThru =  Convert.ToBoolean(reader["passthru"]),
 
            };

            return CodecDef;
        }


        public List<CodecDef> GetCodecDefs()
        {
            String cmdText = "SELECT  codec_id,fs_name,description,clock_rate, default_ms_per_packet,passthru FROM codec_defs;";
            return GetItemsText(cmdText, CodecDefMapper, (cmd) =>
            {
            });
        }
    }
}
