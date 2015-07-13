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
            public abstract string Extension { get; set; }

            public abstract string Directory { get; set; }

            public abstract string ServerIP { get; set; }

            public abstract string UserName { get; set; }

            public abstract string Password { get; set; }

            public abstract string DirectorytoMoveFile { get; set; }

            public abstract Actions ActionAfterImport { get; set; }

        # endregion 


    }
}
