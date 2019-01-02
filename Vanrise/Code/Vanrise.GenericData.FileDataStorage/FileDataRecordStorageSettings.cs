using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.FileDataStorage
{
    public enum FileDataRecordStorageFolderStructureType { MonthDateHour = 1 }
    public class FileDataRecordStorageSettings : DataRecordStorageSettings
    {
        public string RootFolderPath { get; set; }

        public FileDataRecordStorageFolderStructureType? FolderStructureType { get; set; }

        public TimeSpan StoragePartitionInterval { get; set; }

        public string MetadataSchemaName { get; set; }

        public string MetadataTableName { get; set; }

        public bool GenerateRecordIds { get; set; }

        public char FieldSeparator { get; set; }
    }
}
