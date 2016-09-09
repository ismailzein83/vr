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
        Ftp _ftp;
        public FTPSourceReaderSetting Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {
            _ftp = new Ftp();
            string mask = string.IsNullOrEmpty(Setting.Mask) ? "" : Setting.Mask;
            Regex regEx = new Regex(mask);

            EstablishConnection();
            if (_ftp.GetConnectionState().Connected)
            {
                if (!_ftp.DirectoryExists(Setting.Directory))
                    throw new DirectoryNotFoundException();

                _ftp.ChangeDirectory(Setting.Directory);
                FtpList currentItems = _ftp.GetList(string.Format("*{0}", Setting.Extension));

                if (currentItems.Count > 0)
                {
                    foreach (var fileObj in currentItems)
                    {
                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            String filePath = Setting.Directory + "/" + fileObj.Name;
                            context.OnSourceBEBatchRetrieved(GetFileSourceBatch(_ftp, fileObj, filePath), null);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("FTP adapter could not connect to FTP Server");
            }
        }

        void EstablishConnection()
        {
            _ftp.Connect(Setting.ServerIp);
            _ftp.Login(Setting.UserName, Setting.Password);
        }
        void CloseConnection()
        {
            _ftp.Dispose();
        }

        FileSourceBatch GetFileSourceBatch(Ftp ftp, FtpItem fileObj, String filePath)
        {
            var stream = new MemoryStream();
            ftp.GetFile(filePath, stream);
            byte[] content = stream.ToArray();
            return new FileSourceBatch
              {
                  FileName = fileObj.Name,
                  Content = content
              };
        }

        public override void SetBatchCompleted(ISourceBEReaderSetBatchImportedContext context)
        {
            CloseConnection();
            base.SetBatchCompleted(context);
        }
    }

    public class FTPSourceReaderSetting
    {
        public string Extension { get; set; }
        public string Mask { get; set; }
        public string Directory { get; set; }
        public string ServerIp { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
