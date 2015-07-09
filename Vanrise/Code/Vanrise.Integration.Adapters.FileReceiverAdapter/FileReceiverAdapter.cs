using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FileReceiveAdapter
{
    public class FileReceiveAdapter : BaseReceiveAdapter
    {
        public string FolderPath { get; set; }

        public override void ImportData(Action<IImportedData> receiveData)
        {
            //foreach (var fileObj in Directory.EnumerateFiles(this.FolderPath, "*.txt"))
            //{
                
            //}

            receiveData(new TextFileImportedData { Content = "Extracted Data" });
        }
    }
}
