using System;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SFTPReceiveAdapter.Arguments
{
    public class SFTPAdapterArgument : BaseAdapterArgument
    {
        public enum CompressionTypes
        {
            GZip,
            Zip
        }
        public enum Actions
        {
            NoAction = -1,
            Rename = 0,
            Delete = 1,
            Move = 2,// Move to Folder
            Copy = 3 // Copy To Folder and Keep the original file,
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
        public string LastImportedFile { get; set; }
        public bool CompressedFiles { get; set; }
        public CompressionTypes CompressionType { get; set; }
        public short? NumberOfFiles { get; set; }

        short? _fileCompletenessCheckInterval = 5;

        /// <summary>
        /// in seconds
        /// </summary>
        public short? FileCompletenessCheckInterval
        {
            get
            {
                return _fileCompletenessCheckInterval;
            }
            set
            {
                _fileCompletenessCheckInterval = value;
            }
        }

        public string InvalidFilesDirectory { get; set; }

        # endregion
    }

    public class SFTPAdapterState : BaseAdapterState
    {
        public DateTime LastRetrievedFileTime { get; set; }
        public string LastRetrievedFileName { get; set; }
    }
}
