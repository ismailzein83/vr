﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        # endregion
    }

    public class FTPAdapterState : BaseAdapterState
    {
        public DateTime LastRetrievedFileTime { get; set; }
    }
}
