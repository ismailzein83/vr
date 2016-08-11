using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rebex.Net;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.MainExtensions.SourceBEReaders
{
    public class FTPSourceReader : SourceBEReader
    {
        public FTPSourceReaderSetting Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {
            var ftp = new Rebex.Net.Ftp();
            string mask = string.IsNullOrEmpty(Setting.Mask) ? "" : Setting.Mask;
            Regex regEx = new Regex(mask);

            EstablishConnection(ftp);
            if (ftp.GetConnectionState().Connected)
            {
                if (!ftp.DirectoryExists(Setting.Directory))
                    throw new DirectoryNotFoundException();

                ftp.ChangeDirectory(Setting.Directory);
                FtpList currentItems = ftp.GetList(string.Format("*{0}", Setting.Extension));

                if (currentItems.Count > 0)
                {
                    foreach (var fileObj in currentItems)
                    {
                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            String filePath = Setting.Directory + "/" + fileObj.Name;
                            GetFileContent(context.OnSourceBEBatchRetrieved, ftp, fileObj, filePath);
                        }
                    }
                }
                CloseConnection(ftp);
            }
            else
            {
                throw new Exception("FTP adapter could not connect to FTP Server");
            }
        }

        void EstablishConnection(Ftp ftp)
        {
            ftp.Connect(Setting.ServerIP);
            ftp.Login(Setting.UserName, Setting.Password);
        }
        static void CloseConnection(Ftp ftp)
        {
            ftp.Dispose();
        }

        void GetFileContent(Action<FileSourceBatch, SourceBEBatchRetrievedContext> fileBatchAction, Ftp ftp, FtpItem fileObj, String filePath)
        {
            var stream = new MemoryStream();
            ftp.GetFile(filePath, stream);
            byte[] content = stream.ToArray();
            FileSourceBatch batch = new FileSourceBatch
            {
                FileName = fileObj.Name,
                Content = content
            };
            fileBatchAction(batch, null);
        }
    }

    public class FTPSourceReaderSetting
    {
        public string Extension { get; set; }
        public string Mask { get; set; }
        public string Directory { get; set; }
        public string ServerIP { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
