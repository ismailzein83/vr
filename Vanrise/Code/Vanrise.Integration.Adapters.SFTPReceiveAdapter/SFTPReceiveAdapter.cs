using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rebex.Net;
using Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SFTPReceiveAdapter
{
    public class SFTPReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            FTPAdapterArgument ftpAdapterArgument = context.AdapterArgument as FTPAdapterArgument;

            FTPAdapterState ftpAdapterState = SaveOrGetAdapterState(context, ftpAdapterArgument);

            var sftp = new Rebex.Net.Sftp();
            string mask = string.IsNullOrEmpty(ftpAdapterArgument.Mask) ? "" : string.Format(ftpAdapterArgument.Mask);
            Regex regEx = new Regex(mask, RegexOptions.IgnoreCase);

            base.LogVerbose("Establishing SFTP Connection");

            EstablishConnection(sftp, ftpAdapterArgument);
            if (sftp.GetConnectionState().Connected)
            {
                base.LogVerbose("SFTP connection is established");

                if (!sftp.DirectoryExists(ftpAdapterArgument.Directory))
                {
                    base.LogError("Could not find Directory {0}", ftpAdapterArgument.Directory);
                    throw new DirectoryNotFoundException();
                }

                sftp.ChangeDirectory(ftpAdapterArgument.Directory);
                SftpItemCollection currentItems = sftp.GetList(string.Format("{0}/*{1}", ftpAdapterArgument.Directory, ftpAdapterArgument.Extension));

                base.LogInformation("{0} files are ready to be imported", currentItems.Count);

                if (currentItems.Count > 0)
                {
                    DateTime? localLastRetrievedFileTime = null;
                    foreach (var fileObj in currentItems.OrderBy(c => c.Modified))
                    {
                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            if (ftpAdapterArgument.BasedOnLastModifiedTime)
                            {
                                if ((!localLastRetrievedFileTime.HasValue || DateTime.Compare(localLastRetrievedFileTime.Value, fileObj.Modified) != 0)
                                    && DateTime.Compare(ftpAdapterState.LastRetrievedFileTime, fileObj.Modified) >= 0)
                                    continue;
                            }
                            String filePath = ftpAdapterArgument.Directory + "/" + fileObj.Name;
                            CreateStreamReader(context.OnDataReceived, sftp, fileObj, filePath);
                            AfterImport(sftp, fileObj, filePath, ftpAdapterArgument);
                            if (ftpAdapterState.LastRetrievedFileTime != fileObj.Modified)
                            {
                                ftpAdapterState = SaveOrGetAdapterState(context, ftpAdapterArgument, fileObj.Modified);
                                localLastRetrievedFileTime = fileObj.Modified;
                            }
                        }
                    }
                }
                CloseConnection(sftp);
            }
            else
            {
                base.LogError("Could not sftp connect to server {0}", ftpAdapterArgument.ServerIP);
                throw new Exception("SFTP adapter could not connect to SFTP Server");
            }
        }


        #region Private Functions
        private FTPAdapterState SaveOrGetAdapterState(IAdapterImportDataContext context, FTPAdapterArgument ftpAdapterArgument, DateTime? fileModifiedDate = null)
        {
            FTPAdapterState adapterState = null;
            context.GetStateWithLock((state) =>
            {
                adapterState = state as FTPAdapterState;

                if (adapterState == null)
                    adapterState = new FTPAdapterState();

                if (fileModifiedDate != null && fileModifiedDate.HasValue)
                {
                    adapterState.LastRetrievedFileTime = fileModifiedDate.Value;
                }
                return adapterState;
            });

            return adapterState;
        }
        private void CreateStreamReader(Action<IImportedData> receiveData, Sftp sftp, SftpItem fileObj, String filePath)
        {
            base.LogVerbose("Creating stream reader for file with name {0}", fileObj.Name);
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

        private void EstablishConnection(Sftp sftp, FTPAdapterArgument ftpAdapterArgument)
        {
            sftp.Connect(ftpAdapterArgument.ServerIP);
            sftp.Login(ftpAdapterArgument.UserName, ftpAdapterArgument.Password);
        }

        private void AfterImport(Sftp sftp, SftpItem fileObj, String filePath, FTPAdapterArgument ftpAdapterArgument)
        {
            if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Rename)
            {
                base.LogVerbose("Renaming file {0} after import", fileObj.Name);

                sftp.Rename(filePath, string.Format(@"{0}_{1}.processed", filePath.ToLower().Replace(ftpAdapterArgument.Extension.ToLower(), ""), Guid.NewGuid()));
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Delete)
            {
                base.LogVerbose("Deleting file {0} after import", fileObj.Name);

                sftp.DeleteFile(filePath);
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Move)
            {
                base.LogVerbose("Moving file {0} after import", fileObj.Name);

                if (!sftp.DirectoryExists(ftpAdapterArgument.DirectorytoMoveFile))
                    sftp.CreateDirectory(ftpAdapterArgument.DirectorytoMoveFile);

                sftp.Rename(filePath, ftpAdapterArgument.DirectorytoMoveFile + "/" + string.Format(@"{0}_{1}.processed", fileObj.Name.Replace(ftpAdapterArgument.Extension.ToLower(), ""), Guid.NewGuid()));

            }
        }


        #endregion

    }
}
