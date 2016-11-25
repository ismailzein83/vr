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

            FTPSourceReaderState state = context.ReaderState as FTPSourceReaderState;
            if (state == null)
            {
                state = new FTPSourceReaderState();
            }
            using (Ftp ftp = new Ftp())
            {
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
                        DateTime maxFileModifiedTime = state.LastRetrievedFileTime;
                        foreach (var fileObj in currentItems)
                        {

                            if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                            {
                                if (Setting.BasedOnTime && DateTime.Compare(state.LastRetrievedFileTime, fileObj.Modified) >= 0)
                                    continue;
                                String filePath = Setting.Directory + "/" + fileObj.Name;
                                context.OnSourceBEBatchRetrieved(GetFileSourceBatch(ftp, fileObj, filePath), null);
                                maxFileModifiedTime = fileObj.Modified;
                            }
                        }
                        state.LastRetrievedFileTime = maxFileModifiedTime;
                        context.ReaderState = state;
                    }

                }
                else
                {
                    throw new Exception("FTP adapter could not connect to FTP Server");
                }
                CloseConnection(ftp);
            }
        }

        void EstablishConnection(Ftp ftp)
        {
            ftp.Connect(Setting.ServerIp);
            ftp.Login(Setting.UserName, Setting.Password);
        }
        void CloseConnection(Ftp ftp)
        {
            ftp.Dispose();
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

    }

    public class FTPSourceReaderSetting
    {
        public string Extension { get; set; }
        public string Mask { get; set; }
        public string Directory { get; set; }
        public string ServerIp { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool BasedOnTime { get; set; }
    }

    public class FTPSourceReaderState
    {
        public DateTime LastRetrievedFileTime { get; set; }
    }
}
