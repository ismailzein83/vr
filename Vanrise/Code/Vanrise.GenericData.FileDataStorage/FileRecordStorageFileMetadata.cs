﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.FileDataStorage
{
    internal class FileRecordStorageFileMetadata
    {
        public long FileRecordStorageFileMetadataId { get; set; }

        public string ParentFolderRelativePath { get; set; }

        public string FileName { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public DateTime MinTime { get; set; }

        public DateTime MaxTime { get; set; }

        public long MinId { get; set; }

        public long MaxId { get; set; }
    }
}
