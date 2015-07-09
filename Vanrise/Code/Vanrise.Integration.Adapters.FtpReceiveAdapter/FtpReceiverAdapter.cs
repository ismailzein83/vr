using Rebex.Net;
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
        public string Extension { get; set; }

        public string Directory { get; set; }

        public string ServerIP { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool AllowSSH { get; set; }

        public override void ImportData(Action<IImportedData> receiveData)
        {
            
            if (AllowSSH)
            {
                var sftp = new Rebex.Net.Sftp();

                sftp.Connect(ServerIP);
                sftp.Login(Username, Password);
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

                                var stream = new MemoryStream();
                                sftp.GetFile(filePath, stream);
                                byte[] data = stream.ToArray();
                                using (var ms = stream)
                                {
                                    ms.Position = 0;
                                    var sr = new StreamReader(ms);
                                    receiveData(new StreamReaderImportedData() { StreamReader = new StreamReader(ms) });
                                }

                                sftp.Rename(filePath, filePath.Replace(Extension, ".Imported"));
                            }
                        }
                    }
                    sftp.Disconnect();
                }
            }
            else
            {
                var ftp = new Rebex.Net.Ftp();

                ftp.Connect(ServerIP);
                ftp.Login(Username, Password);
                if (ftp.GetConnectionState().Connected)
                {
                    ftp.ChangeDirectory(Directory);
                    FtpList currentItems = ftp.GetList();
                    if (currentItems.Count > 0)
                    {
                        foreach (var fileObj in currentItems)
                        {
                            if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(Extension))
                            {
                                String filePath = Directory + "/" + fileObj.Name;

                                var stream = new MemoryStream();
                                ftp.GetFile(filePath, stream);
                                byte[] data = stream.ToArray();
                                using (var ms = stream)
                                {
                                    ms.Position = 0;
                                    var sr = new StreamReader(ms);
                                    receiveData(new StreamReaderImportedData() { StreamReader = new StreamReader(ms) });
                                }

                                ftp.Rename(filePath, filePath.Replace(Extension, ".Imported"));
                            }
                        }
                    }
                    ftp.Disconnect();
                }
            }
                



            

        }

    }
}
