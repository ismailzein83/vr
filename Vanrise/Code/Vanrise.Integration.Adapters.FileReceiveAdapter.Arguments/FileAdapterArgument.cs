using System;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments
{
    public class FileAdapterArgument : BaseAdapterArgument
    {
        public string Extension { get; set; }

        public string Mask { get; set; }

        public string Directory { get; set; }

        public string DirectorytoMoveFile { get; set; }

        public int ActionAfterImport { get; set; }

        public Guid? FileDataSourceDefinitionId { get; set; }

        public string DuplicatedFilesDirectory { get; set; }

        public override bool IsFileDataSourceDefinitionInUse(Guid fileDataSourceDefinitionId)
        {
            return this.FileDataSourceDefinitionId.HasValue && this.FileDataSourceDefinitionId.Value == fileDataSourceDefinitionId;
        }
    }

    public class FileAdapterState : BaseAdapterState
    {
        public DateTime LastRetrievedFileTime { get; set; }

        public string LastRetrievedFileName { get; set; }
    }
}