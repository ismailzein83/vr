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
        public enum FileCheckCriteriaEnum
        {
            DateAndNameCheck = 0,
            NameCheck = 1,
            None = 2
        }

        #region Properties

        public VRSshParameters SshParameters { get; set; }
        public string Extension { get; set; }
        public string Mask { get; set; }
        public string Directory { get; set; }
        public string ServerIP { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DirectorytoMoveFile { get; set; }
        public int? ActionAfterImport { get; set; }

        FileCheckCriteriaEnum _fileCheckCriteria = FileCheckCriteriaEnum.DateAndNameCheck;
        public FileCheckCriteriaEnum FileCheckCriteria
        {
            get
            {
                return _fileCheckCriteria;
            }
            set
            {
                _fileCheckCriteria = value;
            }
        }
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

        public int _port = 22;
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }
        # endregion
    }

    public class SFTPAdapterState : BaseAdapterState
    {
        public DateTime LastRetrievedFileTime { get; set; }
        public string LastRetrievedFileName { get; set; }
    }

    public class VRSshParameters
    {
        public VRCompressionEnum? Compression { get; set; }
        public VRSshEncryptionAlgorithmEnum? SshEncryptionAlgorithm { get; set; }
        public VRSshHostKeyAlgorithmEnum? SshHostKeyAlgorithm { get; set; }
        public VRSshKeyExchangeAlgorithmEnum? SshKeyExchangeAlgorithm { get; set; }
        public VRSshMacAlgorithmEnum? SshMacAlgorithm { get; set; }
        public VRSshOptionsEnum? SshOptions { get; set; }

        public bool IsEmpty()
        {
            if (!Compression.HasValue && !SshEncryptionAlgorithm.HasValue && !SshHostKeyAlgorithm.HasValue 
             && !SshKeyExchangeAlgorithm.HasValue && !SshMacAlgorithm.HasValue && !SshOptions.HasValue)
                return true;

            return false;
        }
    }

    public enum VRCompressionEnum
    {
        False = 0,
        True = 1
    }

    public enum VRSshEncryptionAlgorithmEnum
    {
        None = 0,
        RC4 = 1,
        TripleDES = 2,
        AES = 4,
        Blowfish = 8,
        Twofish = 16,
        Any = 255
    }

    public enum VRSshHostKeyAlgorithmEnum
    {
        None = 0,
        RSA = 1,
        DSS = 2,
        Any = 255
    }

    public enum VRSshKeyExchangeAlgorithmEnum
    {
        None = 0,
        DiffieHellmanGroup1SHA1 = 1,
        DiffieHellmanGroup14SHA1 = 2,
        DiffieHellmanGroupExchangeSHA1 = 4,
        Any = 255
    }

    public enum VRSshMacAlgorithmEnum
    {
        None = 0,
        MD5 = 1,
        SHA1 = 2,
        Any = 255
    }

    public enum VRSshOptionsEnum
    {
        None = 0
    }
}
