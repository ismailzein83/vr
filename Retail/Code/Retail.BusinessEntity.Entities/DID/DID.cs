using System;
using System.Linq;
using Vanrise.Common;

namespace Retail.BusinessEntity.Entities
{
    public enum DIDNumberType { Number = 0, Range = 1 }//, Prefix = 2 }

    public class DID
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_DID";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("674F8BE5-9F1B-4084-8EE7-4EBE6C8838AE");

        public int DIDId { get; set; }

        public string SourceId { get; set; }

        public DIDSettings Settings { get; set; }
    }

    public class DIDToInsert : DID
    {
        public bool CreateAsSeparate { get; set; }

        public DIDToInsert(string sourceId, DIDSettings settings, bool createAsSeparate)
        {
            SourceId = sourceId;
            Settings = settings;
            CreateAsSeparate = createAsSeparate;
        }
    }

    public class DIDRuntimeEditor
    {
        public DID DID { get; set; }

        public string Description { get; set; }

        public DIDNumberType DIDNumberType { get; set; }
    }
}