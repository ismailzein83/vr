using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCarrierDocumentDataManager : BaseSQLDataManager
    {
        public SourceCarrierDocumentDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCarrierDocument> GetSourceCarrierDocuments()
        {
            return GetItemsText(query_getSourceCarrierDocuments, SourceCarrierDocumentMapper, null);
        }

        private SourceCarrierDocument SourceCarrierDocumentMapper(IDataReader reader)
        {
            SourceCarrierDocument sourceCarrierDocument = new SourceCarrierDocument()
            {
                SourceId = reader["DocumentID"].ToString(),
                ProfileId = (short)reader["ProfileID"],
                Category = reader["Category"] as string,
                Name = reader["Name"] as string,
                Description = reader["Description"] as string,
                Document = GetReaderValue<byte[]>(reader, "Document"),
                Created = GetReaderValue<DateTime>(reader, "Created")
            };
            return sourceCarrierDocument;
        }

        const string query_getSourceCarrierDocuments = @"  SELECT  DocumentID, ProfileID, Name, Description, Category, Document, Created
                                                          FROM  CarrierDocument  WITH (NOLOCK)";
    }
}
