using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.DataParser.Entities;
using Vanrise.Common;
namespace Vanrise.DataParser.Data.SQL
{
    public class ParserTypeDataManager : BaseSQLDataManager, IParserTypeDataManager
    { 
        #region ctor
        public ParserTypeDataManager()
            : base(GetConnectionStringName("DataParserDBConnStringKey", "DataParserDBConnString"))
        {

        }
        #endregion

        public List<Entities.ParserType> GetParserTypes()
        {
            return GetItemsSP("[dataparser].[sp_ParserType_GetAll]", ParserTypeMapper);
        }

        public bool AreParserTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dataparser].[ParserType]", ref updateHandle);
        }

        public bool Update(Entities.ParserType parserType)
        {
            string serializedSettings = parserType.Settings != null ? Vanrise.Common.Serializer.Serialize(parserType.Settings) : null;
            int recordsEffected = ExecuteNonQuerySP("[dataparser].[sp_ParserType_Update]", parserType.ParserTypeId, parserType.Name, serializedSettings);
            return (recordsEffected > 0);
        }

        public bool Insert(ParserType parserType)
        {
            string serializedSettings = parserType.Settings != null ? Vanrise.Common.Serializer.Serialize(parserType.Settings) : null;
            int recordsEffected = ExecuteNonQuerySP("[dataparser].[sp_ParserType_Insert]", parserType.ParserTypeId, parserType.Name,serializedSettings);
            return (recordsEffected > 0);
        }

        #region Mappers

        ParserType ParserTypeMapper(IDataReader reader)
        {
            return new ParserType
            {
               ParserTypeId = (Guid)reader["ID"],
               Name = reader["Name"] as string,
               Settings = Vanrise.Common.Serializer.Deserialize<ParserTypeSettings>(reader["Settings"] as string)
            };
        }

        # endregion
    }
}
