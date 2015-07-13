using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.BaseTP
{
    public abstract class TPReceiveAdapter : BaseReceiveAdapter
    {

        public enum Actions
        {
            Rename = 0,
            Delete = 1,
            Move = 2 // Move to Folder
        }

        #region Properties
        public string Extension { get; set; }

        public string Directory { get; set; }

        public string ServerIP { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string DirectorytoMoveFile { get; set; }

        public Actions ActionAfterImport { get; set; }

        # endregion 


    }
}
