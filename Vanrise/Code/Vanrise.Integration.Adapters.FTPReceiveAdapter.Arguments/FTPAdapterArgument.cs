﻿using System;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments
{
    public class FTPAdapterArgument : BaseAdapterArgument
    {
        public enum Actions
        {
            Rename = 0,
            Delete = 1,
            Move = 2,// Move to Folder
            Copy = 3 // Copy To Folder and Keep the original file,
        }

        public enum CompressionTypes
        {
            GZip,
            Zip
        }

        #region Properties
        public string Extension { get; set; }
        public string Mask { get; set; }
        public string Directory { get; set; }
        public string ServerIP { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DirectorytoMoveFile { get; set; }
        public int? ActionAfterImport { get; set; }
        public bool BasedOnLastModifiedTime { get; set; }
        public string LastImportedFile { get; set; }
        public bool CompressedFiles { get; set; }
        public CompressionTypes CompressionType { get; set; }
        public short? NumberOfFiles { get; set; }
        public short? FileCompletenessCheckInterval { get; set; }
        public string InvalidFilesDirectory { get; set; }

        # endregion
    }

    public class FTPAdapterState : BaseAdapterState
    {
        public DateTime LastRetrievedFileTime { get; set; }
        public string LastRetrievedFileName { get; set; }
    }
}
