using Rebex.Net;
using System;
using System.IO;
using Vanrise.Integration.Adapters.BaseTP;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SFTPReceiveAdapter
{
    public class SFTPReceiveAdapter : TPReceiveAdapter
    {
        #region Private Functions

        private static void CreateStreamReader(Action<IImportedData> receiveData, Sftp sftp, SftpItem fileObj, String filePath)
        {
            var stream = new MemoryStream();
            sftp.GetFile(filePath, stream);
            byte[] data = stream.ToArray();
            using (var ms = stream)
            {
                ms.Position = 0;
                var sr = new StreamReader(ms);
                receiveData(new StreamReaderImportedData()
                {
                    StreamReader = new StreamReader(ms),
                    Modified = fileObj.Modified,
                    Name = fileObj.Name,
                    Size = fileObj.Size
                });
            }
        }
        
        private static void CloseConnection(Sftp sftp)
        {
            sftp.Dispose();
        }
      
        private void EstablishConnection(Sftp sftp)
        {
            sftp.Connect(ServerIP);
            sftp.Login(UserName, Password);
        }

        private void AfterImport(Sftp sftp, SftpItem fileObj, String filePath)
        {
            if (ActionAfterImport == Actions.Rename)
            {
                sftp.Rename(filePath, filePath.Replace(Extension, ".Imported"));
            }
            else if (ActionAfterImport == Actions.Delete)
            {
                sftp.DeleteFile(filePath);
            }
            else if (ActionAfterImport == Actions.Move)
            {
                if (!sftp.DirectoryExists(DirectorytoMoveFile))
                    sftp.CreateDirectory(DirectorytoMoveFile);

                sftp.Rename(filePath, DirectorytoMoveFile + "/" + fileObj.Name);
            }
        }

       
        #endregion

        public override void ImportData(Action<IImportedData> receiveData)
        {

            var sftp = new Rebex.Net.Sftp();

            EstablishConnection(sftp);
            if (sftp.GetConnectionState().Connected)
            {
                sftp.ChangeDirectory(Directory);
                SftpItemCollection currentItems = sftp.GetList();
                if (currentItems.Count > 0)
                {
                    foreach (var fileObj in currentItems)
                    {
                        if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(Extension))
                        {
                            String filePath = Directory + "/" + fileObj.Name;
                            CreateStreamReader(receiveData, sftp, fileObj, filePath);
                            AfterImport(sftp, fileObj, filePath);
                        }
                    }
                }
                CloseConnection(sftp);
            }

        }

    }
}
