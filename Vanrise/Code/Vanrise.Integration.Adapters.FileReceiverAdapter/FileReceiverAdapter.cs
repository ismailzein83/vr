using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments;
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

        #region Private Functions

        private void CreateStreamReader(FileAdapterArgument fileAdapterArgument, Action<IImportedData> receiveData, FileInfo file)
        {
            StreamReaderImportedData reader = new StreamReaderImportedData()
            {
                StreamReader = new StreamReader(fileAdapterArgument.Directory + "/" + file.Name),
                Modified = file.LastWriteTime,
                Name = file.Name,
                Size = file.Length
            };
            
            receiveData(reader);
        }

        private void AfterImport(FileAdapterArgument fileAdapterArgument, FileInfo file)
        {
            if (fileAdapterArgument.ActionAfterImport == (int)Actions.Rename)
            {
                file.MoveTo(Path.Combine(file.DirectoryName, file.Name.ToLower().Replace(fileAdapterArgument.Extension.ToLower(), ".Imported")));
            }
            else if (fileAdapterArgument.ActionAfterImport == (int)Actions.Delete)
            {
                file.Delete();
            }
            else if (fileAdapterArgument.ActionAfterImport == (int)Actions.Move)
            {
                if (System.IO.Directory.Exists(fileAdapterArgument.Directory))
                {
                    file.MoveTo(Path.Combine(fileAdapterArgument.DirectorytoMoveFile, file.Name.Replace(fileAdapterArgument.Extension, ".Imported")));
                }
            }
        }

        #endregion
       
        public override void ImportData(BaseAdapterArgument argument, Action<IImportedData> receiveData)
        {
            FileAdapterArgument fileAdapterArgument = argument as FileAdapterArgument;

            if (System.IO.Directory.Exists(fileAdapterArgument.Directory))
            {
                DirectoryInfo d = new DirectoryInfo(fileAdapterArgument.Directory);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*" + fileAdapterArgument.Extension); //Getting Text files
                foreach (FileInfo file in Files)
                {
                    CreateStreamReader(fileAdapterArgument, receiveData, file);
                    AfterImport(fileAdapterArgument, file);

                }
            }
        }
    }
}
