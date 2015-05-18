using Rebex.Net;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;
using System.Configuration;
namespace Vanrise.Fzero.CDRImport.BP.Activities
{

    #region Arguments Classes

    public class ImportCDRsInput
    {
        public BaseQueue<ImportedCDRBatch> OutputQueue { get; set; }

    }

    #endregion

    public class ImportCDRs : BaseAsyncActivity<ImportCDRsInput>
    {

        #region Arguments

        [RequiredArgument]
        public  InArgument<BaseQueue<ImportedCDRBatch>> OutputQueue { get; set; }

        #endregion


        protected override void DoWork(ImportCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            string SFTP_Dir = System.Configuration.ConfigurationManager.AppSettings["SFTP_Dir"].ToString();
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.Start {0}", DateTime.Now);
            var sftp = new Rebex.Net.Sftp();
            sftp.Connect(System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString());
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.Connect {0}", DateTime.Now);
            sftp.Login(System.Configuration.ConfigurationManager.AppSettings["Server_Username"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Server_Pasword"].ToString());
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.Login {0}", DateTime.Now);
            if (sftp.GetConnectionState().Connected)
            {
                // set current directory
                sftp.ChangeDirectory(SFTP_Dir);
                // get items within the current directory
                SftpItemCollection currentItems = sftp.GetList();
                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.GetList {0}", DateTime.Now);
                if (currentItems.Count > 0)
                {
                    foreach (var fileObj in currentItems)
                    {
                        if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(".DAT"))
                        {
                            String filePath = SFTP_Dir + "/" + fileObj.Name;
                            String newFilePath = SFTP_Dir + "/" + fileObj.Name.Replace(".DAT", ".old");

                            var stream = new MemoryStream();
                            sftp.GetFile(filePath, stream);
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.FileStreamedtoMemory {0}", DateTime.Now);
                            byte[] data = stream.ToArray();
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.StreamConvertedtoByte {0}", DateTime.Now);
                            List<CDR> CDRs = new List<CDR>();
                            using (var ms = stream)
                            {
                                ms.Position = 0;
                                var sr = new StreamReader(ms);

                                while (!sr.EndOfStream)
                                {
                                    var i = sr.ReadLine();

                                    CDR cdr = new CDR();
                                    cdr.MSISDN = i.Substring(145, 20).Trim();
                                    cdr.IMSI = i.Substring(125, 20).Trim();
                                    cdr.Destination = i.Substring(198, 20).Trim();
                                    cdr.Call_Class = i.Substring(434, 10).Trim();
                                    cdr.Sub_Type = i.Substring(165, 10).Trim();
                                    cdr.IMEI = i.Substring(105, 20).Trim();
                                    cdr.Cell_Id = i.Substring(252, 22).Trim();
                                    cdr.In_Trunk = i.Substring(414, 20).Trim();
                                    cdr.Out_Trunk = i.Substring(394, 20).Trim();


                                    DateTime ConnectDateTime;
                                    if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", CultureInfo.InvariantCulture,
                                                               DateTimeStyles.None, out ConnectDateTime))
                                        cdr.ConnectDateTime = ConnectDateTime;



                                    int Call_Type = 0;
                                    if (int.TryParse(i.Substring(102, 3).Trim(), out Call_Type))
                                        cdr.Call_Type = Call_Type;

                                    decimal Cell_Latitude;
                                    if (decimal.TryParse(i.Substring(609, 9).Trim(), out Cell_Latitude))
                                        cdr.Cell_Latitude = Cell_Latitude;


                                    decimal DurationInSeconds;
                                    if (decimal.TryParse(i.Substring(588, 10).Trim(), out DurationInSeconds))
                                        cdr.DurationInSeconds = DurationInSeconds;


                                    decimal Up_Volume;
                                    if (decimal.TryParse(i.Substring(609, 9).Trim(), out Up_Volume))
                                        cdr.UpVolume = Up_Volume;


                                    decimal Cell_Longitude;
                                    if (decimal.TryParse(i.Substring(618, 9).Trim(), out Cell_Longitude))
                                        cdr.Cell_Longitude = Cell_Longitude;


                                    decimal Down_Volume;
                                    if (decimal.TryParse(i.Substring(598, 10).Trim(), out Down_Volume))
                                        cdr.DownVolume = Down_Volume;


                                    CDRs.Add(cdr);
                                }
                            }
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.ArrayToList {0}", DateTime.Now);

                            inputArgument.OutputQueue.Enqueue(new ImportedCDRBatch()
                            {
                                cdrs = CDRs
                            });
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.Enqueued {0}", DateTime.Now);
                            sftp.Rename(filePath, newFilePath);
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start ImportCDRs.DoWork.Renamed {0}", DateTime.Now);

                        }
                    }
                }
                sftp.Disconnect();
            }
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End ImportCDRs.DoWork {0}", DateTime.Now);
        }

        //protected override void DoWork(ImportCDRsInput inputArgument, AsyncActivityHandle handle)
        //{
        //    string SFTP_Dir = System.Configuration.ConfigurationManager.AppSettings["SFTP_Dir"].ToString();
        //    var sftp = new Rebex.Net.Sftp();
        //    sftp.Connect(System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString());
        //    sftp.Login(System.Configuration.ConfigurationManager.AppSettings["Server_Username"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Server_Pasword"].ToString());
        //    if (sftp.GetConnectionState().Connected)
        //    {
        //        sftp.ChangeDirectory(SFTP_Dir);
        //        SftpItemCollection currentItems = sftp.GetList();
        //        if (currentItems.Count > 0)
        //        {
        //            foreach (var fileObj in currentItems)
        //            {

        //                String filePath = SFTP_Dir + "/" + fileObj.Name;
        //                String newFilePath = SFTP_Dir + "/" + fileObj.Name.Replace(".old", ".DAT");
        //                try { sftp.Rename(filePath, newFilePath); }
        //                catch { }

        //            }
        //        }
        //        sftp.Disconnect();
        //    }
        //}

        protected override ImportCDRsInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new ImportCDRsInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
