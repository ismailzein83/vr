﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rebex.Net;
using Vanrise.Common;
using Vanrise.Integration.Adapters.SFTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SFTPReceiveAdapter
{
    public class SFTPReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            SFTPAdapterArgument SFTPAdapterArgument = context.AdapterArgument as SFTPAdapterArgument;

            SFTPAdapterState SFTPAdapterState = SaveOrGetAdapterState(context, SFTPAdapterArgument);

            var sftp = new Rebex.Net.Sftp();
            string mask = string.IsNullOrEmpty(SFTPAdapterArgument.Mask) ? "" : string.Format(SFTPAdapterArgument.Mask);
            Regex regEx = new Regex(mask, RegexOptions.IgnoreCase);

            base.LogVerbose("Establishing SFTP Connection");

            EstablishConnection(sftp, SFTPAdapterArgument);
            if (sftp.GetConnectionState().Connected)
            {
                base.LogVerbose("SFTP connection is established");

                if (!sftp.DirectoryExists(SFTPAdapterArgument.Directory))
                {
                    base.LogError("Could not find Directory {0}", SFTPAdapterArgument.Directory);
                    throw new DirectoryNotFoundException();
                }

                sftp.ChangeDirectory(SFTPAdapterArgument.Directory);
                SftpItemCollection currentItems = sftp.GetList(string.Format("{0}/*{1}", SFTPAdapterArgument.Directory, SFTPAdapterArgument.Extension));

                base.LogInformation("{0} files are ready to be imported", currentItems.Count);

                if (currentItems.Count > 0)
                {
                    DateTime? localLastRetrievedFileTime = null;
                    foreach (var fileObj in currentItems.OrderBy(c => c.Modified).ThenBy(c => c.Name))
                    {
                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            if (SFTPAdapterArgument.BasedOnLastModifiedTime)
                            {
                                if ((!localLastRetrievedFileTime.HasValue || DateTime.Compare(localLastRetrievedFileTime.Value, fileObj.Modified) != 0)
                                    && DateTime.Compare(SFTPAdapterState.LastRetrievedFileTime, fileObj.Modified) >= 0)
                                    continue;
                            }

                            if (!string.IsNullOrEmpty(SFTPAdapterArgument.LastImportedFile) && SFTPAdapterArgument.LastImportedFile.CompareTo(fileObj.Name) >= 0)
                                continue;

                            String filePath = SFTPAdapterArgument.Directory + "/" + fileObj.Name;
                            if (SFTPAdapterState.LastRetrievedFileTime != fileObj.Modified)
                            {
                                SFTPAdapterState = SaveOrGetAdapterState(context, SFTPAdapterArgument, fileObj.Modified);
                                localLastRetrievedFileTime = fileObj.Modified;
                            }

                            CreateStreamReader(context.OnDataReceived, sftp, fileObj, filePath, SFTPAdapterArgument);
                            AfterImport(sftp, fileObj, filePath, SFTPAdapterArgument);
                        }
                    }
                }
                CloseConnection(sftp);
            }
            else
            {
                base.LogError("Could not sftp connect to server {0}", SFTPAdapterArgument.ServerIP);
                throw new Exception("SFTP adapter could not connect to SFTP Server");
            }
        }


        #region Private Functions
        private SFTPAdapterState SaveOrGetAdapterState(IAdapterImportDataContext context, SFTPAdapterArgument SFTPAdapterArgument, DateTime? fileModifiedDate = null)
        {
            SFTPAdapterState adapterState = null;
            context.GetStateWithLock((state) =>
            {
                adapterState = state as SFTPAdapterState;

                if (adapterState == null)
                    adapterState = new SFTPAdapterState();

                if (fileModifiedDate != null && fileModifiedDate.HasValue)
                {
                    adapterState.LastRetrievedFileTime = fileModifiedDate.Value;
                }
                return adapterState;
            });

            return adapterState;
        }
        private void CreateStreamReader(Action<IImportedData> receiveData, Sftp sftp, SftpItem fileObj, String filePath, SFTPAdapterArgument argument)
        {
            base.LogVerbose("Creating stream reader for file with name {0}", fileObj.Name);
            var stream = new MemoryStream();
            sftp.GetFile(filePath, stream);

            stream.Seek(0, SeekOrigin.Begin);

            using (var ms = GetStream(stream, argument.CompressedFiles, argument.CompressionType))
            {
                ms.Position = 0;
                receiveData(new StreamReaderImportedData()
                {
                    Stream = ms,
                    Modified = fileObj.Modified,
                    Name = fileObj.Name,
                    Size = fileObj.Size
                });
            }
        }

        MemoryStream GetStream(MemoryStream stream, bool isCompressed, Vanrise.Integration.Adapters.SFTPReceiveAdapter.Arguments.SFTPAdapterArgument.CompressionTypes compressionType)
        {
            if (isCompressed)
            {
                switch (compressionType)
                {
                    case Vanrise.Integration.Adapters.SFTPReceiveAdapter.Arguments.SFTPAdapterArgument.CompressionTypes.GZip:
                        return new MemoryStream(ZipUtility.DecompressGZ(stream));
                    case Vanrise.Integration.Adapters.SFTPReceiveAdapter.Arguments.SFTPAdapterArgument.CompressionTypes.Zip:
                        return new MemoryStream(ZipUtility.UnZip(stream.ToArray()));
                }
            }

            return stream;
        }
        private static void CloseConnection(Sftp sftp)
        {
            sftp.Dispose();
        }

        private void EstablishConnection(Sftp sftp, SFTPAdapterArgument SFTPAdapterArgument)
        {
            sftp.Connect(SFTPAdapterArgument.ServerIP);
            sftp.Login(SFTPAdapterArgument.UserName, SFTPAdapterArgument.Password);
        }

        private void AfterImport(Sftp sftp, SftpItem fileObj, String filePath, SFTPAdapterArgument SFTPAdapterArgument)
        {
            if (SFTPAdapterArgument.ActionAfterImport == (int)SFTPAdapterArgument.Actions.Rename)
            {
                base.LogVerbose("Renaming file {0} after import", fileObj.Name);

                sftp.Rename(filePath, string.Format(@"{0}.processed", filePath.Replace(SFTPAdapterArgument.Extension, "")));
            }
            else if (SFTPAdapterArgument.ActionAfterImport == (int)SFTPAdapterArgument.Actions.Delete)
            {
                base.LogVerbose("Deleting file {0} after import", fileObj.Name);

                sftp.DeleteFile(filePath);
            }
            else if (SFTPAdapterArgument.ActionAfterImport == (int)SFTPAdapterArgument.Actions.Move)
            {
                base.LogVerbose("Moving file {0} after import", fileObj.Name);

                if (!sftp.DirectoryExists(SFTPAdapterArgument.DirectorytoMoveFile))
                    sftp.CreateDirectory(SFTPAdapterArgument.DirectorytoMoveFile);

                sftp.Rename(filePath, SFTPAdapterArgument.DirectorytoMoveFile + "/" + string.Format(@"{0}.processed", fileObj.Name.Replace(SFTPAdapterArgument.Extension, "")));

            }
        }


        #endregion

    }
}
