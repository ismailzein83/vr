using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FtpReceiverAdapter
{
    public class FileReceiveAdapter : BaseReceiveAdapter
    {
        public string FolderPath { get; set; }

        public override void ImportData(Action<object> receiveData)
        {
            //foreach (var fileObj in Directory.EnumerateFiles(this.FolderPath, "*.txt"))
            //{
                
            //}

            receiveData("Extracted Data");
        }
    }
}
