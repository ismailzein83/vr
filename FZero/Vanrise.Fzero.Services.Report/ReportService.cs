﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using Vanrise.Fzero.Bypass;
using Microsoft.Reporting.WebForms;
using Vanrise.CommonLibrary;
using System.Net;
using System.Text;
using System.Linq;
using Rebex.Net;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;

namespace Vanrise.Fzero.Services.Report
{
    public partial class ReportService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public ReportService()
        {
            InitializeComponent();
        }

        private void ErrorLog(string message)
        {
            string cs = "Report Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        protected override void OnStart(string[] args)
        {
            base.RequestAdditionalTime(15000); // 10 minutes timeout for startup
            //Debugger.Launch(); // launch and attach debugger
            int timeInterval;
            bool parsed = Int32.TryParse(ConfigurationManager.AppSettings["TimeInterval"], out timeInterval);

            aTimer = new System.Timers.Timer(timeInterval);// 2 hours
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = timeInterval;// 2 hours
            aTimer.Enabled = true;
            GC.KeepAlive(aTimer);
            OnTimedEvent(null, null);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                if (HttpHelper.CheckInternetConnection("mail.vanrise.com", 26))
                {
                    foreach (MobileOperator i in MobileOperator.GetMobileOperators())
                    {
                        if (i.AutoReport && i.User.ClientID != null)
                        {
                            HashSet<string> DistinctCLIs = new HashSet<string>();
                            HashSet<string> DistinctCLIsWitoutCountryCode = new HashSet<string>();

                            List<ViewGeneratedCall> listFraudCases = GeneratedCall.GetFraudCases(i.User.ClientID, i.ID);
                            List<int> listDistinctFraudCases = new List<int>();
                            List<int> listRepeatedFraudCases = new List<int>();
                            foreach (ViewGeneratedCall v in listFraudCases)
                            {
                                string cli = v.CLI;
                                string cliWithoutCountryCode = v.CLI;

                                if (i.User.ClientID == (int)Enums.Clients.ITPC)
                                {
                                    if (cli.StartsWith("0"))
                                    {
                                        cli = string.Format("964{0}", v.CLI.Substring(1));
                                        cliWithoutCountryCode = string.Format("{0}", v.CLI.Substring(1));
                                    }
                                }

                                if (!DistinctCLIsWitoutCountryCode.Contains(cli))
                                {
                                    DistinctCLIsWitoutCountryCode.Add(cliWithoutCountryCode);
                                }

                                if (!DistinctCLIs.Contains(cli))
                                {
                                    DistinctCLIs.Add(cli);

                                    DistinctCLIsWitoutCountryCode.Add(cliWithoutCountryCode);

                                    listDistinctFraudCases.Add(v.ID);
                                }
                                else
                                {
                                    listRepeatedFraudCases.Add(v.ID);
                                }
                            }


                            if (listRepeatedFraudCases.Count > 0)
                            {
                                GeneratedCall.UpdateReportStatus(listRepeatedFraudCases, (int)Enums.ReportingStatuses.Ignored, null);
                            }

                            ErrorLog("Count: " + listDistinctFraudCases.Count);
                            if (listDistinctFraudCases.Count > 0)
                            {
                                GeneratedCall.UpdateReportStatus(listDistinctFraudCases, (int)Enums.ReportingStatuses.TobeReported, null);
                                string reportCounter;
                                SendReport(DistinctCLIs, listDistinctFraudCases, i.User.FullName, (int)Enums.Statuses.Fraud, i.ID, i.User.EmailAddress, i.User.ClientID.Value, (i.User.GMT - SysParameter.Global_GMT), out reportCounter);
                                if (i.EnableFTP.HasValue && i.EnableFTP.Value)
                                {
                                    SaveToFTPFile(i.ID,i.User.ClientID.Value,i.FTPAddress, i.FTPUserName, i.FTPPassword, i.FTPPort, i.FTPType, i.User.FullName, DistinctCLIsWitoutCountryCode, reportCounter, i.Compression, i.SshEncryptionAlgorithm, i.SshHostKeyAlgorithm, i.SshKeyExchangeAlgorithm, i.SshMacAlgorithm, i.SshOptions);
                                }
                            }
                        }

                        if (i.AutoReportSecurity && i.User.ClientID != null)
                        {
                            HashSet<string> DistinctCLIs = new HashSet<string>();
                            List<ViewGeneratedCall> listFraudCases = GeneratedCall.GetFraudCasesSecurity(i.User.ClientID, i.ID);
                            List<int> listDistinctFraudCases = new List<int>();
                            List<int> listRepeatedFraudCases = new List<int>();
                            foreach (ViewGeneratedCall v in listFraudCases)
                            {
                                string cli = v.CLI;
                                if (i.User.ClientID == (int)Enums.Clients.ITPC)
                                {
                                    if (cli.StartsWith("0"))
                                    {
                                        cli = string.Format("964{0}", v.CLI.Substring(1));
                                    }
                                }

                                if (!DistinctCLIs.Contains(cli))
                                {
                                    DistinctCLIs.Add(cli);
                                    listDistinctFraudCases.Add(v.ID);
                                }
                                else
                                {
                                    listRepeatedFraudCases.Add(v.ID);
                                }
                            }


                            if (listRepeatedFraudCases.Count > 0)
                            {
                                GeneratedCall.UpdateReportStatusSecurity(listRepeatedFraudCases, (int)Enums.ReportingStatuses.Ignored, null);
                            }

                            ErrorLog("Count: " + listDistinctFraudCases.Count);
                            if (listDistinctFraudCases.Count > 0)
                            {
                                GeneratedCall.UpdateReportStatusSecurity(listDistinctFraudCases, (int)Enums.ReportingStatuses.TobeReported, null);

                                SendReportSecurity(DistinctCLIs, listDistinctFraudCases, i.User.FullName, (int)Enums.Statuses.Fraud, i.ID, i.AutoReportSecurityEmail, i.User.ClientID.Value, (i.User.GMT - SysParameter.Global_GMT));
                            }


                        }
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorLog("OnTimedEvent: " + ex.Message);
                ErrorLog("OnTimedEvent: " + ex.InnerException);
                ErrorLog("OnTimedEvent: " + ex.ToString());
            }



        }

        private void SaveToFTPFile(int mobileOperatorId, int clientId, string ftpAddress, string ftpUserName, string ftpPassword, string ftpPort, int? ftpType, string mobileOperatorName, HashSet<string> distinctCLIs, string reportCounter, int? compression, int? sshEncryptionAlgorithm, int? sshHostKeyAlgorithm, int? sshKeyExchangeAlgorithm, int? sshMacAlgorithm, int? sshOptions)
        {

            try
            {
                var db1 = GetConnectionDB();
                var cmd1 = db1.GetStoredProcCommand("dbo.[sp_FTPReports_GetNotSentReportToRetry]");
                cmd1.CommandTimeout = 600;
                using (cmd1)
                {
                    int maxNumberOfRetry;
                    if (!int.TryParse(ConfigurationManager.AppSettings["FTPMaxNumberOfRetry"], out maxNumberOfRetry))
                        maxNumberOfRetry = 5;

                    db1.AssignParameters(cmd1, new Object[] { mobileOperatorId, clientId, maxNumberOfRetry });
                    var reader = db1.ExecuteReader(cmd1);
                    while (reader.Read())
                    {

                        var reportName = reader["ReportName"] as string;
                        byte[] content = reader["Content"] != DBNull.Value ? (byte[])reader["Content"] : default(byte[]);
                        TrySaveToFTPFile((long)reader["ID"], mobileOperatorId, clientId, reportName, content, ftpAddress, ftpUserName, ftpPassword, ftpPort, ftpType, mobileOperatorName, compression, sshEncryptionAlgorithm, sshHostKeyAlgorithm, sshKeyExchangeAlgorithm, sshMacAlgorithm, sshOptions);
                    }
                    cmd1.Dispose();
                }

                if (reportCounter == null)
                    return;

                string fileName = string.Format("AutoSuspend_{0:yyyyMMdd}_{1}_Vanrise.tmp", DateTime.Now, reportCounter);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in distinctCLIs)
                {
                    stringBuilder.AppendLine(item.ToString());
                }
                long ftpreportId;
                byte[] buffer = new ASCIIEncoding().GetBytes(stringBuilder.ToString());
                var db = GetConnectionDB();
                var cmd = db.GetStoredProcCommand("dbo.[sp_FTPReports_InsertReport]");
                cmd.CommandTimeout = 600;
                using (cmd)
                {
                    db.AssignParameters(cmd, new Object[] { fileName, buffer, false, clientId, mobileOperatorId, 0 });
                    ftpreportId =Convert.ToInt64(db.ExecuteScalar(cmd));
                    cmd.Dispose();
                }
                TrySaveToFTPFile( ftpreportId,mobileOperatorId, clientId, fileName, buffer, ftpAddress, ftpUserName, ftpPassword, ftpPort, ftpType, mobileOperatorName, compression, sshEncryptionAlgorithm, sshHostKeyAlgorithm, sshKeyExchangeAlgorithm, sshMacAlgorithm, sshOptions);
            }
            catch (Exception ex)
            {
                ErrorLog("SaveToFTPFile: " + ex.Message);
                ErrorLog("SaveToFTPFile: " + ex.InnerException);
                ErrorLog("SaveToFTPFile: " + ex.ToString());
            }

        }

        private void TrySaveToFTPFile(long ftpreportId, int mobileOperatorId, int clientId, string fileName, byte[] buffer, string ftpAddress, string ftpUserName, string ftpPassword, string ftpPort, int? ftpType, string mobileOperatorName, int? compression, int? sshEncryptionAlgorithm, int? sshHostKeyAlgorithm, int? sshKeyExchangeAlgorithm, int? sshMacAlgorithm, int? sshOptions)
        {
            try
            {
                if (ftpAddress != null && ftpUserName != null & ftpPassword != null)
                {

                    if (!ftpType.HasValue || ftpType.Value == (int)FTPTypeEnum.FTP)
                    {
                        var ftpWebRequest = Rebex.Net.FtpWebRequest.Create(String.Format("ftp://{0}/{1}", ftpAddress, fileName.Replace(".tmp", ".txt")));
                        ftpWebRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                        //   ftpWebRequest..KeepAlive = false;
                        ftpWebRequest.Timeout = 20000;
                        ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                        //  ftpWebRequest.UseBinary = true;
                        Stream stream = ftpWebRequest.GetRequestStream();
                        ftpWebRequest.ContentLength = buffer.Length;
                        stream.Write(buffer, 0, buffer.Length);
                        int contentLen = buffer.Length;
                        stream.Close();
                        stream.Dispose();
                    }
                    else if (ftpType.Value == (int)FTPTypeEnum.SFTP)
                    {
                        Sftp client = new Sftp();
                        int serverPort = 22;
                        if (ftpPort != null)
                        {
                            serverPort = Convert.ToInt32(ftpPort);
                        }
                        var ftpAddressArray = ftpAddress.Split('/');

                        SshParameters sshParameters = new SshParameters();
                        if (compression.HasValue)
                        {
                            TrySetCompression(sshParameters, (VRCompressionEnum)compression.Value);
                        }
                        if (sshEncryptionAlgorithm.HasValue)
                        {
                            TrySetSshEncryptionAlgorithm(sshParameters, (VRSshEncryptionAlgorithmEnum)sshEncryptionAlgorithm.Value);
                        }
                        if (sshHostKeyAlgorithm.HasValue)
                        {
                            TrySetSshHostKeyAlgorithm(sshParameters, (VRSshHostKeyAlgorithmEnum)sshHostKeyAlgorithm.Value);
                        }
                        if (sshKeyExchangeAlgorithm.HasValue)
                        {
                            TrySetSshKeyExchangeAlgorithm(sshParameters, (VRSshKeyExchangeAlgorithmEnum)sshKeyExchangeAlgorithm.Value);
                        }
                        if (sshMacAlgorithm.HasValue)
                        {
                            TrySetSshMacAlgorithm(sshParameters, (VRSshMacAlgorithmEnum)sshMacAlgorithm.Value);
                        }
                        if (sshOptions.HasValue)
                        {
                            TrySetSshOptions(sshParameters, (VRSshOptionsEnum)sshOptions.Value);
                        }

                        client.Connect(ftpAddressArray[0], serverPort, sshParameters);
                        client.Login(ftpUserName, ftpPassword);
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
                        if (ftpAddressArray.Length > 1)
                        {
                            string filePath = "";
                            for (var i = 1; i < ftpAddressArray.Length; i++)
                            {
                                filePath += string.Format("/{0}", ftpAddressArray[i]);
                            }
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                fileName = string.Format("{0}/{1}", filePath, fileName);
                            }
                        }
                        bool isTransferDone = false;
                        string remotePath = null;
                        client.TransferProgress += (sender1, e1) =>
                        {
                            if (e1.Finished)
                            {
                                isTransferDone = true;
                                remotePath = e1.RemotePath;
                            }
                        };
                        client.PutFile(ms, fileName);
                        while (!isTransferDone)
                        {
                            System.Threading.Thread.Sleep(200);
                        }
                        client.Rename(remotePath, remotePath.Replace(".tmp", ".txt"));

                        var db = GetConnectionDB();
                        var cmd = db.GetStoredProcCommand("dbo.[sp_FTPReports_SetReportSent]");
                        cmd.CommandTimeout = 600;
                        using (cmd)
                        {
                            db.AssignParameters(cmd, new Object[] { ftpreportId, fileName.Replace(".tmp", ".txt") });
                            db.ExecuteNonQuery(cmd);
                            cmd.Dispose();
                        }

                        client.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                var db = GetConnectionDB();
                var cmd = db.GetStoredProcCommand("dbo.[sp_FTPReports_UpdateRetryCount]");
                cmd.CommandTimeout = 600;
                using (cmd)
                {
                    db.AssignParameters(cmd, new Object[] { ftpreportId, ex.ToString() });
                    db.ExecuteNonQuery(cmd);
                    cmd.Dispose();
                }
                ErrorLog(string.Format("SaveToFTPFile: Unable To Send {0}.", fileName));
                ErrorLog("SaveToFTPFile: " + ex.Message);
                ErrorLog("SaveToFTPFile: " + ex.InnerException);
                ErrorLog("SaveToFTPFile: " + ex.ToString());
            }
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

        private SqlDatabase GetConnectionDB()
        {
            return new SqlDatabase(ConfigurationManager.ConnectionStrings["FMSConnectionString"].ConnectionString);
        }

        private void TrySetSshEncryptionAlgorithm(SshParameters sshParameters, VRSshEncryptionAlgorithmEnum? sshEncryptionAlgorithm)
        {
            if (sshEncryptionAlgorithm.HasValue)
            {
                switch (sshEncryptionAlgorithm.Value)
                {
                    case VRSshEncryptionAlgorithmEnum.AES: sshParameters.EncryptionAlgorithms = Rebex.Net.SshEncryptionAlgorithm.AES; break;
                    case VRSshEncryptionAlgorithmEnum.Any: sshParameters.EncryptionAlgorithms = Rebex.Net.SshEncryptionAlgorithm.Any; break;
                    case VRSshEncryptionAlgorithmEnum.Blowfish: sshParameters.EncryptionAlgorithms = Rebex.Net.SshEncryptionAlgorithm.Blowfish; break;
                    case VRSshEncryptionAlgorithmEnum.None: sshParameters.EncryptionAlgorithms = Rebex.Net.SshEncryptionAlgorithm.None; break;
                    case VRSshEncryptionAlgorithmEnum.RC4: sshParameters.EncryptionAlgorithms = Rebex.Net.SshEncryptionAlgorithm.RC4; break;
                    case VRSshEncryptionAlgorithmEnum.TripleDES: sshParameters.EncryptionAlgorithms = Rebex.Net.SshEncryptionAlgorithm.TripleDES; break;
                    case VRSshEncryptionAlgorithmEnum.Twofish: sshParameters.EncryptionAlgorithms = Rebex.Net.SshEncryptionAlgorithm.Twofish; break;
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
                    case VRSshHostKeyAlgorithmEnum.Any: sshParameters.HostKeyAlgorithms = Rebex.Net.SshHostKeyAlgorithm.Any; break;
                    case VRSshHostKeyAlgorithmEnum.DSS: sshParameters.HostKeyAlgorithms = Rebex.Net.SshHostKeyAlgorithm.DSS; break;
                    case VRSshHostKeyAlgorithmEnum.None: sshParameters.HostKeyAlgorithms = Rebex.Net.SshHostKeyAlgorithm.None; break;
                    case VRSshHostKeyAlgorithmEnum.RSA: sshParameters.HostKeyAlgorithms = Rebex.Net.SshHostKeyAlgorithm.RSA; break;
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
                    case VRSshKeyExchangeAlgorithmEnum.Any: sshParameters.KeyExchangeAlgorithms = Rebex.Net.SshKeyExchangeAlgorithm.Any; break;
                    case VRSshKeyExchangeAlgorithmEnum.DiffieHellmanGroup14SHA1: sshParameters.KeyExchangeAlgorithms = Rebex.Net.SshKeyExchangeAlgorithm.DiffieHellmanGroup14SHA1; break;
                    case VRSshKeyExchangeAlgorithmEnum.DiffieHellmanGroup1SHA1: sshParameters.KeyExchangeAlgorithms = Rebex.Net.SshKeyExchangeAlgorithm.DiffieHellmanGroup1SHA1; break;
                    case VRSshKeyExchangeAlgorithmEnum.DiffieHellmanGroupExchangeSHA1: sshParameters.KeyExchangeAlgorithms = Rebex.Net.SshKeyExchangeAlgorithm.DiffieHellmanGroupExchangeSHA1; break;
                    case VRSshKeyExchangeAlgorithmEnum.None: sshParameters.KeyExchangeAlgorithms = Rebex.Net.SshKeyExchangeAlgorithm.None; break;
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
                    case VRSshMacAlgorithmEnum.Any: sshParameters.MacAlgorithms = Rebex.Net.SshMacAlgorithm.Any; break;
                    case VRSshMacAlgorithmEnum.MD5: sshParameters.MacAlgorithms = Rebex.Net.SshMacAlgorithm.MD5; break;
                    case VRSshMacAlgorithmEnum.None: sshParameters.MacAlgorithms = Rebex.Net.SshMacAlgorithm.None; break;
                    case VRSshMacAlgorithmEnum.SHA1: sshParameters.MacAlgorithms = Rebex.Net.SshMacAlgorithm.SHA1; break;
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
                    case VRSshOptionsEnum.None: sshParameters.Options = Rebex.Net.SshOptions.None; break;
                    default: throw new NotSupportedException(string.Format("VRSshOptionsEnum {0} not supported.", sshOptions.Value));
                }
            }
        }

        private void SendReport(HashSet<string> CLIs, List<int> ListIds, string MobileOperatorName, int StatusID, int MobileOperatorID, string EmailAddress, int ClientID, int DifferenceInGMT, out string reportCounter)
        {
            reportCounter = null;
            try
            {
                ReportViewer rvToOperator = new ReportViewer();
                ReportViewer rvToOperator2 = new ReportViewer();
                Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();

                report.SentDateTime = DateTime.Now;

                if (ClientID == 3) //ST
                {
                    report.RecommendedAction = "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.";
                    report.ApplicationUserID = 8;
                }
                else if (ClientID == 9) //Etisalat 
                {
                    report.RecommendedAction = "It is highly recommended to immediately block and take actions against these fraudulent MSISDNs as they are terminating international calls without passing legally through Etisalat IGW.";
                    report.ApplicationUserID = 3;
                }
                else
                {
                    report.RecommendedAction = "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.";
                    report.ApplicationUserID = 3;
                }

                ErrorLog("ClientID: " + ClientID);

                string ReportID;

                string ReportIDBeforeCounter = "FZ" + MobileOperatorName.Substring(0, 1) + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");


                Vanrise.Fzero.Bypass.Report LastReport = Vanrise.Fzero.Bypass.Report.Load(ReportIDBeforeCounter);

                string counter;
                if (LastReport == null)
                {
                    counter = "0001";
                   
                }
                else
                {
                    counter = (int.Parse(LastReport.ReportID.Replace("- Repeated Numbers", "").Substring(10)) + 1).ToString("D4");
                }
                reportCounter = counter;
                ReportID = ReportIDBeforeCounter + counter;
              

                ErrorLog("ReportID: " + ReportID);
                report.ReportID = ReportID;


                if (StatusID == (int)Enums.Statuses.Suspect)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
                }
                else if (StatusID == (int)Enums.Statuses.Fraud)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Block;

                }

                Vanrise.Fzero.Bypass.Report reportSaved = Vanrise.Fzero.Bypass.Report.Save(report);
                GeneratedCall.SendReport(ListIds, reportSaved.ID);
                ReportParameter[] parameters = new ReportParameter[3];
                parameters[0] = new ReportParameter("ReportID", report.ReportID);



                if (ClientID == 3)
                {
                    parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.");
                }
                else if (ClientID == 9) //Etisalat 
                {
                    parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately block and take actions against these fraudulent MSISDNs as they are terminating international calls without passing legally through Etisalat IGW.");

                }
                else
                {
                    parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.");
                }



                string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.Report\");
                string reportPath = string.Empty;
                string reportPath2 = string.Empty;


                if (ClientID == (int)Enums.Clients.ST)//-- Syrian Telecom
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToSyrianOperator.rdlc");
                }
                else if (ClientID == (int)Enums.Clients.Zain)//-- Zain
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToZainOperator.rdlc");
                }
                else if (ClientID == (int)Enums.Clients.ITPC)//-- ITPC
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToOperator.rdlc");
                    reportPath2 = Path.Combine(exeFolder, @"Reports\rptToZainOperatorExcel.rdlc");
                    ErrorLog("reportPath: " + reportPath);
                    ErrorLog("reportPath2: " + reportPath2);
                }
                else if (ClientID == (int)Enums.Clients.Madar)//-- Madar
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToMadarOperator.rdlc");
                    ErrorLog("reportPath: " + reportPath);
                }
                else if(ClientID == (int)Enums.Clients.Etisalat)//-- Madar
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToEtisalatOperator.rdlc");
                }
                else
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptDefaultToOperator.rdlc");
                }
                




                rvToOperator.LocalReport.ReportPath = reportPath;
                if (ClientID == (int)Enums.Clients.ITPC)
                    rvToOperator2.LocalReport.ReportPath = reportPath2;


                ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(1)).User.Signature);
                rvToOperator.LocalReport.DataSources.Add(SignatureDataset);


                ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
                rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);

                //
                //Get reported calls compared from generated calls
                //
                List<ViewGeneratedCall> reportedCalls = GeneratedCall.GetReportedCalls(report.ReportID, DifferenceInGMT);
                int reportedCallsCount = reportedCalls.Count;
                if (reportedCallsCount == 0)
                {
                    Vanrise.Fzero.Bypass.Report.Delete(reportSaved);
                }

                ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", reportedCalls);
                rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                if (ClientID == (int)Enums.Clients.ITPC)
                    rvToOperator2.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);




                parameters[2] = new ReportParameter("HideSignature", "true");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
              
                
                
                string filenameExcel = ClientVariables.ExportReportToExcel(report.ReportID + ".xls", rvToOperator);
                string filenameExcel2 = "";
                if (ClientID == (int)Enums.Clients.ITPC)
                {
                    rvToOperator2.LocalReport.SetParameters(parameters);
                    rvToOperator2.LocalReport.Refresh();
                    filenameExcel2 = ClientVariables.ExportReportToExcel(report.ReportID + ".xls", rvToOperator2);
                    ErrorLog("filenameExcel: " + filenameExcel);
                    ErrorLog("filenameExcel2: " + filenameExcel2);
                }
                
                




                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();

                string attachedPath = null;
                MobileOperator mobileOperator = Vanrise.Fzero.Bypass.MobileOperator.Load(MobileOperatorID);
                if (mobileOperator.IncludeCSVFile.HasValue && mobileOperator.IncludeCSVFile.Value)
                {
                    attachedPath = ClientVariables.ExportReportToCSV(report.ReportID + ".csv", rvToOperator);
                }


                string filenamePDF = ClientVariables.ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);

                ErrorLog("filenamePDF: " + filenamePDF);



              

                string CCs = EmailCC.GetEmailCCs(MobileOperatorID, ClientID);

                string profileName = ClientVariables.GetProfileName(ClientID);


             
                if (ClientID == 3)
                {
                    if (attachedPath != null)
                    {
                        attachedPath += ";" + filenameExcel + ";" + filenamePDF;
                    }else
                    {
                        attachedPath = filenameExcel + ";" + filenamePDF;
                    }
                    EmailManager.SendReporttoMobileSyrianOperator(reportedCallsCount, attachedPath,
                                EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                                report.ReportID, profileName);
                }
                else if (ClientID == (int)Enums.Clients.Madar)
                {

                    string filenameCSV = ClientVariables.ExportReportToCSV(report.ReportID + ".csv", rvToOperator);
                    ErrorLog("Madar: filenameCSV " + filenameCSV + " report.ReportID " + report.ReportID);

                    EmailManager.SendReporttoMobileOperator(reportedCallsCount, filenameCSV,
                                EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                                report.ReportID, profileName);
                }
                else
                {
                    string zainExcel = ConfigurationManager.AppSettings["ZainExcel"];

                    ErrorLog("zainExcel: " + zainExcel);

                    if (string.IsNullOrEmpty(zainExcel))
                    {
                        if (attachedPath != null)
                        {
                            attachedPath += ";" +  filenamePDF;
                        }
                        else
                        {
                            attachedPath = filenamePDF;
                        }

                        EmailManager.SendReporttoMobileOperator(reportedCallsCount, attachedPath, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profileName);
                    }
                    else
                    {
                        string reportCode = ReportID.Substring(0, 3);

                        if (reportCode == "FZZ")
                        {
                            ErrorLog("Zain Client");

                            if (zainExcel == "true")
                            {
                                ErrorLog("reportedCallsCount: " + reportedCallsCount + " filenameExcel2: " + filenameExcel2 + " filenamePDF: " + filenamePDF
                                        + " EmailAddress: " + EmailAddress + " report.ReportID: " + report.ReportID);

                                if (attachedPath != null)
                                {
                                    attachedPath += ";" + filenameExcel2 + ";" + filenamePDF;
                                }
                                else
                                {
                                    attachedPath = filenameExcel2 + ";" + filenamePDF;
                                }

                                EmailManager.SendReporttoMobileOperator(reportedCallsCount, attachedPath, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profileName);
                            }
                            else
                            {
                                if (attachedPath != null)
                                {
                                    attachedPath += ";" + filenamePDF;
                                }
                                else
                                {
                                    attachedPath = filenamePDF;
                                }

                                EmailManager.SendReporttoMobileOperator(reportedCallsCount, attachedPath, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profileName);
                            }
                               
                        }
                        else
                        {
                            if (attachedPath != null)
                            {
                                attachedPath += ";"+ filenamePDF;
                            }
                            else
                            {
                                attachedPath = filenamePDF;
                            }


                            EmailManager.SendReporttoMobileOperator(reportedCallsCount, attachedPath, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profileName);
                        }
                    }

                }



                if (mobileOperator.EnableAutoBlock && !string.IsNullOrEmpty(mobileOperator.AutoBlockEmail))
                {
                    EmailManager.SendAutoBlockReport(mobileOperator.AutoBlockEmail, CLIs, report.ReportID, profileName);
                }
            }
            catch (Exception e)
            {
                ErrorLog("SendReport: " + e.Message);
            }


        }

        private void SendReportSecurity(HashSet<string> CLIs, List<int> ListIds, string MobileOperatorName, int StatusID, int MobileOperatorID, string EmailAddress, int ClientID, int DifferenceInGMT)
        {
            try
            {

                ReportViewer rvToOperator = new ReportViewer();
                Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();


                report.SentDateTime = DateTime.Now;

                if (ClientID == 3) //ST
                {
                    report.RecommendedAction = "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.";
                    report.ApplicationUserID = 8;
                }
                else
                {
                    report.RecommendedAction = "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.";
                    report.ApplicationUserID = 3;
                }

                ErrorLog("ClientID: " + ClientID);

                string ReportID;

                string ReportIDBeforeCounter = "FZ" + MobileOperatorName.Substring(0, 1) + "S" + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");


                Vanrise.Fzero.Bypass.Report LastReport = Vanrise.Fzero.Bypass.Report.Load(ReportIDBeforeCounter);
                if (LastReport == null)
                {
                    ReportID = ReportIDBeforeCounter + "0001";
                }
                else
                {
                    ReportID = ReportIDBeforeCounter + (int.Parse(LastReport.ReportID.Replace("- Repeated Numbers", "").Substring(10)) + 1).ToString("D4");
                }
                ErrorLog("ReportID: " + ReportID);
                report.ReportID = ReportID;


                if (StatusID == (int)Enums.Statuses.Suspect)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
                }
                else if (StatusID == (int)Enums.Statuses.Fraud)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Block;

                }

                Vanrise.Fzero.Bypass.Report ReportSaved = Vanrise.Fzero.Bypass.Report.Save(report);
                GeneratedCall.SendReportSecurity(ListIds, ReportSaved.ID);
                ReportParameter[] parameters = new ReportParameter[3];
                parameters[0] = new ReportParameter("ReportID", report.ReportID);



                if (ClientID == 3)
                {
                    parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.");
                }
                else
                {
                    parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.");
                }



                string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.Report\");
                string reportPath = string.Empty;
                reportPath = Path.Combine(exeFolder, @"Reports\rptToOperatorIraqNationalSec.rdlc");
                rvToOperator.LocalReport.ReportPath = reportPath;

                ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(1)).User.Signature);
                rvToOperator.LocalReport.DataSources.Add(SignatureDataset);


                ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
                rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);

                ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", GeneratedCall.GetReportedSecCalls(ReportSaved.ID, DifferenceInGMT));
                rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);

                parameters[2] = new ReportParameter("HideSignature", "true");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenameExcel = ClientVariables.ExportReportToExcel(report.ReportID + ".xls", rvToOperator);

                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenamePDF = ClientVariables.ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);

                ErrorLog("filenamePDF: " + filenamePDF);
                    
                string CCs = EmailCC.GetEmailCCs(MobileOperatorID, ClientID);
                string profileName = ClientVariables.GetProfileName(ClientID);


                EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                    ConfigurationManager.AppSettings["EmailCCNatSec"], report.ReportID, profileName);


            }
            catch (Exception e)
            {
                ErrorLog("SendReport: " + e.Message);
            }


        }


        //private string ExportReportToPDF(string reportName, ReportViewer rvToOperator)
        //{
        //    Warning[] warnings;
        //    string[] streamids;
        //    string mimeType;
        //    string encoding;
        //    string filenameExtension;
        //    byte[] bytes = rvToOperator.LocalReport.Render(
        //       "PDF", null, out mimeType, out encoding, out filenameExtension,
        //        out streamids, out warnings);

        //    string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
        //    ClientVariables.SaveClientReport(filename, bytes);


        //    return filename;
        //}

        //private string ExportReportToExcel(string reportName, ReportViewer rvToOperator)
        //{
        //    Warning[] warnings;
        //    string[] streamids;
        //    string mimeType;
        //    string encoding;
        //    string filenameExtension;
        //    byte[] bytes = rvToOperator.LocalReport.Render(
        //       "Excel", null, out mimeType, out encoding, out filenameExtension,
        //        out streamids, out warnings);

        //    string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
        //    ClientVariables.SaveClientReport(filename, bytes);


        //    return filename;
        //}

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }

    public enum VRCompressionEnum
    {
        False = 0,
        True = 1
    }

    public enum VRSshEncryptionAlgorithmEnum
    {
        None = 0,
        RC4 = 1,
        TripleDES = 2,
        AES = 4,
        Blowfish = 8,
        Twofish = 16,
        Any = 255
    }

    public enum VRSshHostKeyAlgorithmEnum
    {
        None = 0,
        RSA = 1,
        DSS = 2,
        Any = 255
    }

    public enum VRSshKeyExchangeAlgorithmEnum
    {
        None = 0,
        DiffieHellmanGroup1SHA1 = 1,
        DiffieHellmanGroup14SHA1 = 2,
        DiffieHellmanGroupExchangeSHA1 = 4,
        Any = 255
    }

    public enum VRSshMacAlgorithmEnum
    {
        None = 0,
        MD5 = 1,
        SHA1 = 2,
        Any = 255
    }

    public enum VRSshOptionsEnum
    {
        None = 0
    }

}









