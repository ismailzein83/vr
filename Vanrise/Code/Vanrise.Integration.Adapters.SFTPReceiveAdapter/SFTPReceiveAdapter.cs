using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Rebex.Net;
using Vanrise.Common;
using Vanrise.Integration.Adapters.SFTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;
using System.Collections.Generic;

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
                SftpItemCollection sftpCollection = sftp.GetList(string.Format("{0}/*{1}", SFTPAdapterArgument.Directory, SFTPAdapterArgument.Extension));

                if (sftpCollection.Count > 0)
                {
                    short numberOfFilesRead = 0;

                    SftpItemCollection sftpCollectionToProcess = CheckandGetFinalSftpCollection(SFTPAdapterArgument, sftp, sftpCollection);
                    bool newFilesStarted = false;
                    foreach (var fileObj in sftpCollectionToProcess.OrderBy(c => c.Modified).ThenBy(c => c.Name))
                    {
                        if (context.ShouldStopImport())
                            break;
                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            if (!newFilesStarted)
                            {
                                if (DateTime.Compare(SFTPAdapterState.LastRetrievedFileTime, fileObj.Modified) > 0)
                                {
                                    continue;
                                }
                                else if (DateTime.Compare(SFTPAdapterState.LastRetrievedFileTime, fileObj.Modified) == 0)
                                {
                                    if (!string.IsNullOrEmpty(SFTPAdapterState.LastRetrievedFileName) && SFTPAdapterState.LastRetrievedFileName.CompareTo(fileObj.Name) >= 0)
                                        continue;
                                }
                                newFilesStarted = true;
                            }

                            if (!string.IsNullOrEmpty(SFTPAdapterArgument.LastImportedFile) && SFTPAdapterArgument.LastImportedFile.CompareTo(fileObj.Name) >= 0)
                                continue;

                            String filePath = SFTPAdapterArgument.Directory + "/" + fileObj.Name;
                            ImportedBatchProcessingOutput output = CreateStreamReader(context.OnDataReceived, sftp, fileObj, filePath, SFTPAdapterArgument);

                            SFTPAdapterState = SaveOrGetAdapterState(context, SFTPAdapterArgument, fileObj.Name, fileObj.Modified);


                            AfterImport(sftp, fileObj, filePath, SFTPAdapterArgument, output);

                            numberOfFilesRead++;

                            if (SFTPAdapterArgument.NumberOfFiles.HasValue && SFTPAdapterArgument.NumberOfFiles.Value == numberOfFilesRead)
                            {
                                base.LogInformation("Max number of files {0} reached", numberOfFilesRead);
                                break;
                            }
                        }
                    }

                    base.LogInformation("{0} files have been imported", numberOfFilesRead);
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
        SFTPAdapterState SaveOrGetAdapterState(IAdapterImportDataContext context, SFTPAdapterArgument SFTPAdapterArgument, string fileName = null, DateTime? fileModifiedDate = null)
        {
            SFTPAdapterState adapterState = null;
            context.GetStateWithLock((state) =>
            {
                adapterState = state as SFTPAdapterState;

                if (adapterState == null)
                    adapterState = new SFTPAdapterState();

                if (fileModifiedDate.HasValue)
                {
                    adapterState.LastRetrievedFileTime = fileModifiedDate.Value;
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    adapterState.LastRetrievedFileName = fileName;
                }
                return adapterState;
            });

            return adapterState;
        }

        ImportedBatchProcessingOutput CreateStreamReader(Func<IImportedData, ImportedBatchProcessingOutput> receiveData, Sftp sftp, SftpItem fileObj, String filePath, SFTPAdapterArgument argument)
        {
            ImportedBatchProcessingOutput output = null;
            base.LogVerbose("Creating stream reader for file with name {0}", fileObj.Name);
            var stream = new MemoryStream();
            sftp.GetFile(filePath, stream);

            stream.Seek(0, SeekOrigin.Begin);

            using (var ms = GetStream(stream, argument.CompressedFiles, argument.CompressionType))
            {
                ms.Position = 0;
                output = receiveData(new StreamReaderImportedData()
                {
                    Stream = ms,
                    Modified = fileObj.Modified,
                    Name = fileObj.Name,
                    Size = fileObj.Size
                });
            }
            stream.Close();
            return output;
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

        static void CloseConnection(Sftp sftp)
        {
            sftp.Dispose();
        }

        void EstablishConnection(Sftp sftp, SFTPAdapterArgument sftpAdapterArgument)
        {
            SshParameters sshParameters = BuildSshParameters(sftpAdapterArgument.SshParameters);
            if (sshParameters == null)
                sftp.Connect(sftpAdapterArgument.ServerIP, sftpAdapterArgument.Port);
            else
                sftp.Connect(sftpAdapterArgument.ServerIP, sftpAdapterArgument.Port, sshParameters);

            sftp.Login(sftpAdapterArgument.UserName, sftpAdapterArgument.Password);
        }

        SshParameters BuildSshParameters(VRSshParameters vrSshParameters)
        {
            if (vrSshParameters == null || vrSshParameters.IsEmpty())
                return null;

            SshParameters sshParameters = new SshParameters();
            TrySetCompression(sshParameters, vrSshParameters.Compression);
            TrySetSshEncryptionAlgorithm(sshParameters, vrSshParameters.SshEncryptionAlgorithm);
            TrySetSshHostKeyAlgorithm(sshParameters, vrSshParameters.SshHostKeyAlgorithm);
            TrySetSshKeyExchangeAlgorithm(sshParameters, vrSshParameters.SshKeyExchangeAlgorithm);
            TrySetSshMacAlgorithm(sshParameters, vrSshParameters.SshMacAlgorithm);
            TrySetSshOptions(sshParameters, vrSshParameters.SshOptions);

            return sshParameters;
        }

        private void TrySetCompression(SshParameters sshParameters, VRCompressionEnum? compression)
        {
            if (compression.HasValue)
            {
                switch (compression.Value)
                {
                    case VRCompressionEnum.False: sshParameters.Compression = false; break;
                    case VRCompressionEnum.True: sshParameters.Compression = true; break;
                    default: throw new NotSupportedException(string.Format("VRCompressionEnum {0} not supported.", compression.Value));
                }
            }
        }

        private void TrySetSshEncryptionAlgorithm(SshParameters sshParameters, VRSshEncryptionAlgorithmEnum? sshEncryptionAlgorithm)
        {
            if (sshEncryptionAlgorithm.HasValue)
            {
                switch (sshEncryptionAlgorithm.Value)
                {
                    case VRSshEncryptionAlgorithmEnum.AES: sshParameters.EncryptionAlgorithms = SshEncryptionAlgorithm.AES; break;
                    case VRSshEncryptionAlgorithmEnum.Any: sshParameters.EncryptionAlgorithms = SshEncryptionAlgorithm.Any; break;
                    case VRSshEncryptionAlgorithmEnum.Blowfish: sshParameters.EncryptionAlgorithms = SshEncryptionAlgorithm.Blowfish; break;
                    case VRSshEncryptionAlgorithmEnum.None: sshParameters.EncryptionAlgorithms = SshEncryptionAlgorithm.None; break;
                    case VRSshEncryptionAlgorithmEnum.RC4: sshParameters.EncryptionAlgorithms = SshEncryptionAlgorithm.RC4; break;
                    case VRSshEncryptionAlgorithmEnum.TripleDES: sshParameters.EncryptionAlgorithms = SshEncryptionAlgorithm.TripleDES; break;
                    case VRSshEncryptionAlgorithmEnum.Twofish: sshParameters.EncryptionAlgorithms = SshEncryptionAlgorithm.Twofish; break;
                    default: throw new NotSupportedException(string.Format("VRSshEncryptionAlgorithmEnum {0} not supported.", sshEncryptionAlgorithm.Value));
                }
            }
        }

        private void TrySetSshHostKeyAlgorithm(SshParameters sshParameters, VRSshHostKeyAlgorithmEnum? sshHostKeyAlgorithm)
        {
            if (sshHostKeyAlgorithm.HasValue)
            {
                switch (sshHostKeyAlgorithm.Value)
                {
                    case VRSshHostKeyAlgorithmEnum.Any: sshParameters.HostKeyAlgorithms = SshHostKeyAlgorithm.Any; break;
                    case VRSshHostKeyAlgorithmEnum.DSS: sshParameters.HostKeyAlgorithms = SshHostKeyAlgorithm.DSS; break;
                    case VRSshHostKeyAlgorithmEnum.None: sshParameters.HostKeyAlgorithms = SshHostKeyAlgorithm.None; break;
                    case VRSshHostKeyAlgorithmEnum.RSA: sshParameters.HostKeyAlgorithms = SshHostKeyAlgorithm.RSA; break;
                    default: throw new NotSupportedException(string.Format("VRSshHostKeyAlgorithmEnum {0} not supported.", sshHostKeyAlgorithm.Value));
                }
            }
        }

        private void TrySetSshKeyExchangeAlgorithm(SshParameters sshParameters, VRSshKeyExchangeAlgorithmEnum? sshKeyExchangeAlgorithm)
        {
            if (sshKeyExchangeAlgorithm.HasValue)
            {
                switch (sshKeyExchangeAlgorithm.Value)
                {
                    case VRSshKeyExchangeAlgorithmEnum.Any: sshParameters.KeyExchangeAlgorithms = SshKeyExchangeAlgorithm.Any; break;
                    case VRSshKeyExchangeAlgorithmEnum.DiffieHellmanGroup14SHA1: sshParameters.KeyExchangeAlgorithms = SshKeyExchangeAlgorithm.DiffieHellmanGroup14SHA1; break;
                    case VRSshKeyExchangeAlgorithmEnum.DiffieHellmanGroup1SHA1: sshParameters.KeyExchangeAlgorithms = SshKeyExchangeAlgorithm.DiffieHellmanGroup1SHA1; break;
                    case VRSshKeyExchangeAlgorithmEnum.DiffieHellmanGroupExchangeSHA1: sshParameters.KeyExchangeAlgorithms = SshKeyExchangeAlgorithm.DiffieHellmanGroupExchangeSHA1; break;
                    case VRSshKeyExchangeAlgorithmEnum.None: sshParameters.KeyExchangeAlgorithms = SshKeyExchangeAlgorithm.None; break;
                    default: throw new NotSupportedException(string.Format("VRSshKeyExchangeAlgorithmEnum {0} not supported.", sshKeyExchangeAlgorithm.Value));
                }
            }
        }

        private void TrySetSshMacAlgorithm(SshParameters sshParameters, VRSshMacAlgorithmEnum? sshMacAlgorithm)
        {
            if (sshMacAlgorithm.HasValue)
            {
                switch (sshMacAlgorithm.Value)
                {
                    case VRSshMacAlgorithmEnum.Any: sshParameters.MacAlgorithms = SshMacAlgorithm.Any; break;
                    case VRSshMacAlgorithmEnum.MD5: sshParameters.MacAlgorithms = SshMacAlgorithm.MD5; break;
                    case VRSshMacAlgorithmEnum.None: sshParameters.MacAlgorithms = SshMacAlgorithm.None; break;
                    case VRSshMacAlgorithmEnum.SHA1: sshParameters.MacAlgorithms = SshMacAlgorithm.SHA1; break;
                    default: throw new NotSupportedException(string.Format("VRSshMacAlgorithmEnum {0} not supported.", sshMacAlgorithm.Value));
                }
            }
        }

        private void TrySetSshOptions(SshParameters sshParameters, VRSshOptionsEnum? sshOptions)
        {
            if (sshOptions.HasValue)
            {
                switch (sshOptions.Value)
                {
                    case VRSshOptionsEnum.None: sshParameters.Options = SshOptions.None; break;
                    default: throw new NotSupportedException(string.Format("VRSshOptionsEnum {0} not supported.", sshOptions.Value));
                }
            }
        }

        void AfterImport(Sftp sftp, SftpItem fileObj, String filePath, SFTPAdapterArgument SFTPAdapterArgument, ImportedBatchProcessingOutput output)
        {
            if (output != null && output.OutputResult.Result == MappingResult.Invalid && !string.IsNullOrEmpty(SFTPAdapterArgument.InvalidFilesDirectory))
            {
                MoveFile(sftp, fileObj, filePath, SFTPAdapterArgument.InvalidFilesDirectory, SFTPAdapterArgument.Extension, "invalid");

            }
            else if (SFTPAdapterArgument.ActionAfterImport == (int)SFTPAdapterArgument.Actions.Rename)
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
                MoveFile(sftp, fileObj, filePath, SFTPAdapterArgument.DirectorytoMoveFile, SFTPAdapterArgument.Extension, "processed");
            }
        }

        void MoveFile(Sftp sftp, SftpItem fileObj, String filePath, string directorytoMoveFile, string extension, string newExtension)
        {
            base.LogVerbose("Moving file {0} after import to Directory {1}", fileObj.Name, directorytoMoveFile);

            if (!sftp.DirectoryExists(directorytoMoveFile))
                sftp.CreateDirectory(directorytoMoveFile);

            sftp.Rename(filePath, directorytoMoveFile + "/" + string.Format(@"{0}.{1}", fileObj.Name.Replace(extension, ""), newExtension));
        }

        SftpItemCollection CheckandGetFinalSftpCollection(SFTPAdapterArgument SFTPAdapterArgument, Sftp sftp, SftpItemCollection sftpCollection)
        {
            SftpItemCollection sftpCollectionToProcess = new SftpItemCollection();
            if (SFTPAdapterArgument.FileCompletenessCheckInterval.HasValue)
            {
                Dictionary<string, SftpItem> firstReadFilesByName = sftpCollection.ToDictionary(itm => itm.Name, itm => itm);
                Thread.Sleep(SFTPAdapterArgument.FileCompletenessCheckInterval.Value * 1000);
                SftpItemCollection currenctSftpCollection = sftp.GetList(string.Format("{0}/*{1}", SFTPAdapterArgument.Directory, SFTPAdapterArgument.Extension));
                foreach (var ftpFile in currenctSftpCollection.OrderBy(c => c.Modified))
                {
                    SftpItem fileReadFirstTime = firstReadFilesByName.GetRecord(ftpFile.Name);
                    if (fileReadFirstTime == null || fileReadFirstTime.Size != ftpFile.Size)
                        break;

                    sftpCollectionToProcess.Add(ftpFile);
                }
            }
            else
            {
                sftpCollectionToProcess = sftpCollection;
            }
            return sftpCollectionToProcess;
        }

        #endregion

    }
}
