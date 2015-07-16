using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments
{
    public class FileAdapterArgument : BaseAdapterArgument
    {
        public string Extension { get; set; }

        public string Directory { get; set; }

        public string DirectorytoMoveFile { get; set; }

        public int ActionAfterImport { get; set; }
    }
}
