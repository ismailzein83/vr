﻿using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.Data.SQL
{
    public class ParserTypeDataManager : BaseSQLDataManager, IParserTypeDataManager
    {
        #region Properties/Ctor

        public ParserTypeDataManager()
            : base(GetConnectionStringName("DataParserDBConnStringKey", "DataParserDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<Entities.ParserType> GetParserTypes()
        {
            return GetItemsSP("[dataparser].[sp_ParserType_GetAll]", ParserTypeMapper);
        }

        public bool AreParserTypesUpdated(ref object lastReceivedDataInfo)
        {
            return base.IsDataUpdated("[dataparser].[ParserType]", ref lastReceivedDataInfo);
        }

        public bool Insert(ParserType parserType)
        {
            string serializedSettings = parserType.Settings != null ? Vanrise.Common.Serializer.Serialize(parserType.Settings) : null;
            int recordsEffected = ExecuteNonQuerySP("[dataparser].[sp_ParserType_Insert]", parserType.ParserTypeId, parserType.Name, parserType.DevProjectId, serializedSettings);
            return (recordsEffected > 0);
        }

        public bool Update(Entities.ParserType parserType)
        {
            string serializedSettings = parserType.Settings != null ? Vanrise.Common.Serializer.Serialize(parserType.Settings) : null;
            int recordsEffected = ExecuteNonQuerySP("[dataparser].[sp_ParserType_Update]", parserType.ParserTypeId, parserType.Name, parserType.DevProjectId, serializedSettings);
            return (recordsEffected > 0);
        }

        #endregion

        #region Mappers

        ParserType ParserTypeMapper(IDataReader reader)
        {
            return new ParserType
            {
                ParserTypeId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                DevProjectId = GetReaderValue<Guid?>(reader, "DevProjectID"),
                Settings = Vanrise.Common.Serializer.Deserialize<ParserTypeSettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}