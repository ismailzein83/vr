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

        public enum Actions
        {
            Rename = 0,
            Delete = 1,
            Move = 2 // Move to Folder
        }

        #region Properties
        public string Extension { get; set; }

        public string Directory { get; set; }

        public string DirectorytoMoveFile { get; set; }

        public int ActionAfterImport { get; set; }

        # endregion 

        #region Private Functions

        private void CreateStreamReader(Action<IImportedData> receiveData, FileInfo file)
        {
            receiveData(new StreamReaderImportedData()
            {
                StreamReader = new StreamReader(this.Directory + "/" + file.Name),
                Modified = file.LastWriteTime,
                Name = file.Name,
                Size = file.Length
            });
        }

        private void AfterImport(FileInfo file)
        {
            if (ActionAfterImport == (int)Actions.Rename)
            {
                file.MoveTo(Path.Combine(file.DirectoryName, file.Name.Replace(this.Extension,".Imported")));
            }
            else if (ActionAfterImport == (int)Actions.Delete)
            {
                file.Delete();
            }
            else if (ActionAfterImport == (int)Actions.Move)
            {
                if (System.IO.Directory.Exists(this.Directory))
                {
                    file.MoveTo(Path.Combine(DirectorytoMoveFile, file.Name.Replace(this.Extension, ".Imported")));
                }
            }
        }

        #endregion


       
        public override void ImportData(Action<IImportedData> receiveData)
        {
            if (System.IO.Directory.Exists(this.Directory))
            {
                DirectoryInfo d = new DirectoryInfo(this.Directory);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.DAT"); //Getting Text files
                foreach (FileInfo file in Files)
                {
                    CreateStreamReader(receiveData, file);
                    AfterImport(file);

                }
            }
        }
    }
}
