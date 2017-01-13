using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass
{
    public partial class GeneratedCall
    {
        public static bool SendReport(List<int> ListIds, int ReportID)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    foreach (int id in ListIds)
                    {
                        foreach (GeneratedCall generatedCall in context.GeneratedCalls.Where(x => x.ID == id).ToList())
                        {
                            generatedCall.ReportID = ReportID;
                            generatedCall.MobileOperatorFeedbackID = (int)Enums.MobileOperatorFeedbacks.Pending;
                            generatedCall.ReportingStatusID = (int)Enums.ReportingStatuses.Reported;
                            context.Entry(generatedCall).State = System.Data.EntityState.Modified;
                        }
                    }

                    context.SaveChanges();
                }



                List<CasesLog> ListCasesLogs = new List<CasesLog>();

                foreach (int ID in ListIds)
                {
                    CasesLog cl1 = new CasesLog();
                    cl1.UpdatedOn = DateTime.Now;
                    cl1.ChangeTypeID = (int)Enums.ChangeType.ChangedStatus;
                    cl1.GeneratedCallID = ID;
                    cl1.ReportingStatusID = (int)Enums.ReportingStatuses.Reported;
                    ListCasesLogs.Add(cl1);
                }

                CasesLog.SaveBulk("CasesLogs", ListCasesLogs);


                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.UserPermission.Report(" + ListIds.Count.ToString() + ")", err);
            }
            return success;




        }

        public static bool SendReportSecurity(List<int> ListIds, int ReportID)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    foreach (int id in ListIds)
                    {
                        foreach (GeneratedCall generatedCall in context.GeneratedCalls.Where(x => x.ID == id).ToList())
                        {
                            generatedCall.ReportSecID = ReportID;
                            generatedCall.ReportingStatusSecurityID = (int)Enums.ReportingStatuses.Reported;
                            context.Entry(generatedCall).State = System.Data.EntityState.Modified;
                        }
                    }

                    context.SaveChanges();
                }



                List<CasesLog> ListCasesLogs = new List<CasesLog>();

                foreach (int ID in ListIds)
                {
                    CasesLog cl1 = new CasesLog();
                    cl1.UpdatedOn = DateTime.Now;
                    cl1.ChangeTypeID = (int)Enums.ChangeType.ChangedStatus;
                    cl1.GeneratedCallID = ID;
                    cl1.ReportingStatusID = (int)Enums.ReportingStatuses.Reported;
                    ListCasesLogs.Add(cl1);
                }

                CasesLog.SaveBulk("CasesLogs", ListCasesLogs);


                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SendReportSecurity.Report(" + ListIds.Count.ToString() + ")", err);
            }
            return success;




        }

        public static void Confirm(int SourceID, DataTable dt, int? ImportedBy)
        {
            HashSet<string> CountryCodes = new HashSet<string>();


            List<Client> ListClients = Client.GetAllClients();

            foreach (var i in ListClients)
                CountryCodes.Add(i.CountryCode);


            List<Carrier> ListCarriers = Vanrise.Fzero.Bypass.Carrier.GetAllCarriers();
            List<OriginationNetwork> ListOriginationNetworks = Vanrise.Fzero.Bypass.OriginationNetwork.GetAllOriginationNetworks();




            List<Carrier> ListNewCarriers = new List<Carrier>();
            List<OriginationNetwork> ListNewOriginationNetworks = new List<OriginationNetwork>();


            List<CasesLog> ListCasesLogs = new List<CasesLog>();
            List<int> listIDs = new List<int>();



            int Global_GMT = SysParameter.Global_GMT;
            string Global_DefaultMobileOperator = SysParameter.Global_DefaultMobileOperator;

            List<MobileOperator> lstMobileOperators = MobileOperator.GetMobileOperators();
            Source source = Source.Load(SourceID);
            Import import = new Import();
            import.ImportDate = DateTime.Now;
            import.ImportedBy = ImportedBy;
            import.ImportTypeId = (int)Enums.ImportTypes.GeneratedCalls;
            Import.Save(import);


            switch (source.SourceTypeID)
            {



                case (int)Enums.SourceTypes.GeneratesandRecieves:


                    // Find Difference in GMT
                    int GMTDifferenceGeneratesandRecieves = 0;
                    GMTDifferenceGeneratesandRecieves = Global_GMT - source.GMT;

                    int CounterGeneratesandRecieves = 1;
                    List<GeneratedCall> listGeneratesandRecievesGeneratedCalls = new List<GeneratedCall>();
                    foreach (DataRow i in dt.Rows)
                    {
                        GeneratedCall gc = new GeneratedCall();
                        float DurationInSeconds = 0; ///1111111
                        if (i.Table.Columns.Contains("Duration In Seconds"))
                        {
                            if (i["Duration In Seconds"].ToString() != string.Empty)
                            {
                                float.TryParse(i["Duration In Seconds"].ToString(), out DurationInSeconds);///1111111
                            }
                        }

                        gc.DurationInSeconds = Convert.ToInt32(DurationInSeconds);





                        string a_number = string.Empty;
                        if (i.Table.Columns.Contains("a_number"))
                        {
                            a_number = i["a_number"].ToString();
                        }
                        gc.a_number = a_number;


                        string b_number = string.Empty;
                        if (i.Table.Columns.Contains("b_number"))
                        {
                            b_number = i["b_number"].ToString();
                        }
                        gc.b_number = b_number;

                        string CLI = string.Empty;
                        if (i.Table.Columns.Contains("CLI"))
                        {
                            CLI = i["CLI"].ToString();
                        }
                        gc.CLI = CLI;



                        string Type = "SIP";
                        if (i.Table.Columns.Contains("Type"))
                        {
                            Type = i["Type"].ToString();
                        }
                        gc.Type = Type;



                        string Cleanb_number = gc.b_number;

                        if (gc.b_number.StartsWith("+"))
                        {
                            Cleanb_number = gc.b_number.Substring(1);
                        }

                        else if (gc.b_number.StartsWith("00"))
                        {
                            Cleanb_number = gc.b_number.Substring(2);
                        }



                        foreach (MobileOperator j in lstMobileOperators)
                        {
                            if (j.User.FullName == Global_DefaultMobileOperator)
                            {
                                gc.MobileOperatorID = j.ID;
                            }
                        }


                        foreach (MobileOperator j in lstMobileOperators)
                        {
                            List<string> Prefixes = j.User.Prefix.Split(';').ToList<string>();

                            foreach (string p in Prefixes)
                            {
                                if (p != string.Empty)
                                {
                                    int result = 0;
                                    if (int.TryParse(p, out result))
                                    {
                                        if (Cleanb_number.StartsWith(ListClients.Where(x => x.ID == j.User.ClientID.Value).First().CountryCode + p))
                                        {
                                            gc.MobileOperatorID = j.ID;
                                        }
                                    }


                                }
                            }
                        }


                        string Reference = string.Empty;
                        if (i.Table.Columns.Contains("Reference"))
                        {
                            Reference = i["Reference"].ToString();
                        }
                        gc.Reference = Reference;


                        string Carrier = string.Empty;
                        if (i.Table.Columns.Contains("Carrier"))
                        {
                            Carrier = i["Carrier"].ToString().ToUpper();
                        }
                        gc.Carrier = Carrier;



                        string OriginationNetwork = string.Empty;
                        if (i.Table.Columns.Contains("Origination Network"))
                        {
                            OriginationNetwork = i["Origination Network"].ToString().ToUpper();
                        }
                        gc.OriginationNetwork = OriginationNetwork;



                        DateTime AttemptDateTime = new DateTime();
                        if (i.Table.Columns.Contains("Attempt Date Time"))
                        {
                            if (i["Attempt Date Time"].ToString() != string.Empty)
                            {
                                DateTime.TryParse(i["Attempt Date Time"].ToString(), out AttemptDateTime);
                                // Apply Difference in GMT
                                AttemptDateTime = AttemptDateTime.AddHours(GMTDifferenceGeneratesandRecieves);
                            }
                        }
                        gc.AttemptDateTime = AttemptDateTime;


                        gc.StatusID = (int)Enums.Statuses.Pending;
                        gc.ImportID = import.ID;
                        gc.ReportingStatusID = (int)Enums.ReportingStatuses.Pending;
                        gc.ReportingStatusSecurityID = (int)Enums.ReportingStatuses.Pending;
                        gc.SourceID = SourceID;
                        CounterGeneratesandRecieves++;



                        if (AttemptDateTime != DateTime.Parse("1/1/0001 12:00:00 AM"))
                        {
                            listGeneratesandRecievesGeneratedCalls.Add(gc);

                        }



                    }
                    GeneratedCall.SaveBulk("GeneratedCalls", listGeneratesandRecievesGeneratedCalls);




                    using (Entities context = new Entities())
                    {
                        listGeneratesandRecievesGeneratedCalls = context.GeneratedCalls.OrderByDescending(x => x.ID).Take(listGeneratesandRecievesGeneratedCalls.Count()).OrderBy(x => x.ID).ToList();
                        listIDs = listGeneratesandRecievesGeneratedCalls.OrderByDescending(x => x.ID).Take(listGeneratesandRecievesGeneratedCalls.Count()).OrderBy(x => x.ID).Select(x => x.ID).ToList();

                    }


                    foreach (int ID in listIDs)
                    {
                        CasesLog cl1 = new CasesLog();
                        cl1.UpdatedOn = DateTime.Now;
                        cl1.ChangeTypeID = (int)Enums.ChangeType.ChangedStatus;
                        cl1.GeneratedCallID = ID;
                        cl1.StatusID = (int)Enums.Statuses.Pending;
                        ListCasesLogs.Add(cl1);
                    }







                    try
                    {
                        foreach (var i in listGeneratesandRecievesGeneratedCalls)
                        {
                            if (ListCarriers.Where(x => x.Name == i.Carrier.ToUpper()).Count() == 0)
                            {
                                if (ListNewCarriers.Where(x => x.Name == i.Carrier.ToUpper()).Count() == 0)
                                {
                                    Carrier TempCarrier = new Carrier();
                                    TempCarrier.Name = i.Carrier.ToUpper();
                                    ListNewCarriers.Add(TempCarrier);
                                }
                            }
                        }

                        Vanrise.Fzero.Bypass.Carrier.SaveBulk("Carriers", ListNewCarriers);


                        foreach (var i in listGeneratesandRecievesGeneratedCalls)
                        {
                            if (ListOriginationNetworks.Where(x => x.Name == i.OriginationNetwork.ToUpper()).Count() == 0)
                            {
                                if (ListNewOriginationNetworks.Where(x => x.Name == i.OriginationNetwork.ToUpper()).Count() == 0)
                                {
                                    OriginationNetwork TempOriginationNetwork = new OriginationNetwork();
                                    TempOriginationNetwork.Name = i.OriginationNetwork.ToUpper();
                                    ListNewOriginationNetworks.Add(TempOriginationNetwork);
                                }
                            }
                        }

                        Vanrise.Fzero.Bypass.OriginationNetwork.SaveBulk("OriginationNetworks", ListNewOriginationNetworks);
                    }
                    catch
                    {
                    }








                    // Find Difference in GMT
                    int GMTDifferenceReceivesOnly = 0;
                    GMTDifferenceReceivesOnly = Global_GMT - source.GMT;

                    int CounterReceivesOnly = 1;
                    List<RecievedCall> listReceivedCalls = new List<RecievedCall>();
                    foreach (DataRow i in dt.Rows)
                    {
                        RecievedCall rc = new RecievedCall();
                        float DurationInSeconds = 0;
                        if (i.Table.Columns.Contains("Duration In Seconds"))
                        {
                            if (i["Duration In Seconds"].ToString() != string.Empty)
                            {
                                float.TryParse(i["Duration In Seconds"].ToString(), out DurationInSeconds);
                            }
                        }

                        rc.DurationInSeconds = Convert.ToInt32(DurationInSeconds); ;///1111111
                        ///


                        int ClientID = 1; ///ITPC
                        if (i.Table.Columns.Contains("ClientName"))
                        {
                            if (i["ClientName"].ToString().Trim() != string.Empty)
                            {
                                if (ListClients.Where(x => x.Name == i["ClientName"].ToString().Trim()).Count() > 0)
                                {
                                    ClientID = ListClients.Where(x => x.Name == i["ClientName"].ToString().Trim()).FirstOrDefault().ID;
                                }
                            }
                        }

                        rc.ClientID = ClientID;




                        string a_number = string.Empty;
                        if (i.Table.Columns.Contains("a_number"))
                        {
                            a_number = i["a_number"].ToString();
                        }
                        rc.a_number = a_number;


                        string b_number = string.Empty;
                        if (i.Table.Columns.Contains("b_number"))
                        {
                            b_number = i["b_number"].ToString();
                        }
                        rc.b_number = b_number;

                        string CLI = string.Empty;
                        if (i.Table.Columns.Contains("CLI"))
                        {
                            CLI = i["CLI"].ToString();
                        }
                        rc.CLI = CLI;

                        string Type = "SIP";
                        if (i.Table.Columns.Contains("Type"))
                        {
                            Type = i["Type"].ToString();
                        }
                        rc.Type = Type;

                        string Reference = string.Empty;
                        if (i.Table.Columns.Contains("Reference"))
                        {
                            Reference = i["Reference"].ToString();
                        }
                        rc.Reference = Reference;


                        string Carrier = string.Empty;
                        if (i.Table.Columns.Contains("Carrier"))
                        {
                            Carrier = i["Carrier"].ToString().ToUpper();
                        }
                        rc.Carrier = Carrier;



                        string OriginationNetwork = string.Empty;
                        if (i.Table.Columns.Contains("Origination Network"))
                        {
                            OriginationNetwork = i["Origination Network"].ToString().ToUpper();
                        }
                        rc.OriginationNetwork = OriginationNetwork;








                        // Add Area Code to be Able to Know the Mobile Operator Prefix
                        string NumberWithoutAreaCodeCLI = rc.CLI;


                        if (rc.CLI.StartsWith("+"))
                        {
                            NumberWithoutAreaCodeCLI = rc.CLI.Substring(1);
                        }

                        if (rc.CLI.StartsWith("00"))
                        {
                            NumberWithoutAreaCodeCLI = rc.CLI.Substring(2);
                        }

                        else if (rc.CLI.StartsWith("0"))
                        {
                            NumberWithoutAreaCodeCLI = rc.CLI.Substring(1);
                        }










                        foreach (var code in CountryCodes)
                        {
                            if (rc.CLI.StartsWith(code))
                            {
                                NumberWithoutAreaCodeCLI = rc.CLI.Substring(code.Count());
                            }

                            if (rc.CLI.StartsWith("+" + code))
                            {
                                NumberWithoutAreaCodeCLI = rc.CLI.Substring(code.Count() + 1);
                            }

                            if (rc.CLI.StartsWith("00" + code))
                            {
                                NumberWithoutAreaCodeCLI = rc.CLI.Substring(code.Count() + 2);
                            }
                        }




                        rc.MobileOperatorID = null;


                        foreach (MobileOperator j in lstMobileOperators)
                        {
                            List<string> Prefixes = j.User.Prefix.Split(';').ToList<string>();

                            foreach (string p in Prefixes)
                            {
                                if (p != string.Empty)
                                {
                                    int result = 0;
                                    if (int.TryParse(p, out result))
                                    {
                                        if (NumberWithoutAreaCodeCLI.StartsWith(p))
                                        {
                                            rc.MobileOperatorID = j.ID;
                                        }
                                    }


                                }
                            }
                        }



                        DateTime AttemptDateTime = new DateTime();
                        if (i.Table.Columns.Contains("Attempt Date Time"))
                        {
                            if (i["Attempt Date Time"].ToString() != string.Empty)
                            {
                                DateTime.TryParse(i["Attempt Date Time"].ToString(), out AttemptDateTime);
                                // Apply Difference in GMT
                                AttemptDateTime = AttemptDateTime.AddHours(GMTDifferenceGeneratesandRecieves);
                            }
                        }
                        rc.AttemptDateTime = AttemptDateTime;


                        rc.ImportID = import.ID;
                        rc.SourceID = SourceID;
                        CounterReceivesOnly++;

                        if (AttemptDateTime != DateTime.Parse("1/1/0001 12:00:00 AM"))
                        {
                            listReceivedCalls.Add(rc);

                        }
                    }


                    for (int j = 0; j < listGeneratesandRecievesGeneratedCalls.Count; j++)
                    {
                        listReceivedCalls[j].GeneratedCallID = listGeneratesandRecievesGeneratedCalls[j].ID;
                    }

                    RecievedCall.SaveBulk("RecievedCalls", listReceivedCalls);




                    break;









                case (int)Enums.SourceTypes.GeneratesOnly:




                    // Find Difference in GMT
                    int GMTDifferenceGeneratesOnly = 0;
                    GMTDifferenceGeneratesOnly = Global_GMT - source.GMT;

                    int CounterGeneratesOnly = 1;
                    List<GeneratedCall> listGeneratedCalls = new List<GeneratedCall>();
                    foreach (DataRow i in dt.Rows)
                    {
                        GeneratedCall gc = new GeneratedCall();
                        float DurationInSeconds = 0;
                        if (i.Table.Columns.Contains("Duration In Seconds"))
                        {
                            if (i["Duration In Seconds"].ToString() != string.Empty)
                            {
                                float.TryParse(i["Duration In Seconds"].ToString(), out DurationInSeconds);
                            }
                        }

                        gc.DurationInSeconds = Convert.ToInt32(DurationInSeconds); ;///1111111


                        string a_number = string.Empty;
                        if (i.Table.Columns.Contains("a_number"))
                        {
                            a_number = i["a_number"].ToString();
                        }
                        gc.a_number = a_number;


                        string b_number = string.Empty;
                        if (i.Table.Columns.Contains("b_number"))
                        {
                            b_number = i["b_number"].ToString();
                        }
                        gc.b_number = b_number;

                        string CLI = string.Empty;
                        if (i.Table.Columns.Contains("CLI"))
                        {
                            CLI = i["CLI"].ToString();
                        }
                        gc.CLI = CLI;


                        string Type = "SIP";
                        if (i.Table.Columns.Contains("Type"))
                        {
                            Type = i["Type"].ToString();
                        }
                        gc.Type = Type;







                        // Add Area Code to be Able to Know the Mobile Operator Prefix
                        string cleanb_number = gc.b_number;

                        if (gc.b_number.StartsWith("+"))
                        {
                            cleanb_number = gc.b_number.Substring(1);
                        }

                        else if (gc.b_number.StartsWith("00"))
                        {
                            cleanb_number = gc.b_number.Substring(2);
                        }


                        foreach (MobileOperator j in lstMobileOperators)
                        {
                            if (j.User.FullName == Global_DefaultMobileOperator)
                            {
                                gc.MobileOperatorID = j.ID;
                            }
                        }


                        foreach (MobileOperator j in lstMobileOperators)
                        {
                            List<string> Prefixes = j.User.Prefix.Split(';').ToList<string>();

                            foreach (string p in Prefixes)
                            {
                                if (p != string.Empty)
                                {
                                    int result = 0;
                                    if (int.TryParse(p, out result))
                                    {
                                        if (cleanb_number.StartsWith(ListClients.Where(x => x.ID == j.User.ClientID.Value).First().CountryCode + p))
                                        {
                                            gc.MobileOperatorID = j.ID;
                                        }
                                    }


                                }
                            }
                        }


                        string Reference = string.Empty;
                        if (i.Table.Columns.Contains("Reference"))
                        {
                            Reference = i["Reference"].ToString();
                        }
                        gc.Reference = Reference;

                        string Carrier = string.Empty;
                        if (i.Table.Columns.Contains("Carrier"))
                        {
                            Carrier = i["Carrier"].ToString().ToUpper();
                        }
                        gc.Carrier = Carrier;


                        string OriginationNetwork = string.Empty;
                        if (i.Table.Columns.Contains("Origination Network"))
                        {
                            OriginationNetwork = i["Origination Network"].ToString().ToUpper();
                        }
                        gc.OriginationNetwork = OriginationNetwork;


                        DateTime AttemptDateTime = new DateTime();
                        if (i.Table.Columns.Contains("Attempt Date Time"))
                        {
                            if (i["Attempt Date Time"].ToString() != string.Empty)
                            {
                                DateTime.TryParse(i["Attempt Date Time"].ToString(), out AttemptDateTime);
                                // Apply Difference in GMT
                                AttemptDateTime = AttemptDateTime.AddHours(GMTDifferenceGeneratesOnly);
                            }
                        }
                        gc.AttemptDateTime = AttemptDateTime;


                        gc.StatusID = (int)Enums.Statuses.Pending;
                        gc.ImportID = import.ID;
                        gc.ReportingStatusID = (int)Enums.ReportingStatuses.Pending;
                        gc.ReportingStatusSecurityID = (int)Enums.ReportingStatuses.Pending;
                        gc.SourceID = SourceID;
                        CounterGeneratesOnly++;

                        if (AttemptDateTime != DateTime.Parse("1/1/0001 12:00:00 AM"))
                        {
                            listGeneratedCalls.Add(gc);

                        }


                    }
                    GeneratedCall.SaveBulk("GeneratedCalls", listGeneratedCalls);


                    using (Entities context = new Entities())
                    {
                        listIDs = context.GeneratedCalls.OrderByDescending(x => x.ID).Take(listGeneratedCalls.Count()).Select(x => x.ID).ToList();
                    }


                    foreach (int ID in listIDs)
                    {
                        CasesLog cl1 = new CasesLog();
                        cl1.UpdatedOn = DateTime.Now;
                        cl1.ChangeTypeID = (int)Enums.ChangeType.ChangedStatus;
                        cl1.GeneratedCallID = ID;
                        cl1.StatusID = (int)Enums.Statuses.Pending;
                        ListCasesLogs.Add(cl1);
                    }



                    try
                    {
                        foreach (var i in listGeneratedCalls)
                        {
                            if (ListCarriers.Where(x => x.Name == i.Carrier.ToUpper()).Count() == 0)
                            {
                                if (ListNewCarriers.Where(x => x.Name == i.Carrier.ToUpper()).Count() == 0)
                                {
                                    Carrier TempCarrier = new Carrier();
                                    TempCarrier.Name = i.Carrier.ToUpper();
                                    ListNewCarriers.Add(TempCarrier);
                                }
                            }
                        }

                        Vanrise.Fzero.Bypass.Carrier.SaveBulk("Carriers", ListNewCarriers);


                        foreach (var i in listGeneratedCalls)
                        {
                            if (ListOriginationNetworks.Where(x => x.Name == i.OriginationNetwork.ToUpper()).Count() == 0)
                            {
                                if (ListNewOriginationNetworks.Where(x => x.Name == i.OriginationNetwork.ToUpper()).Count() == 0)
                                {
                                    OriginationNetwork TempOriginationNetwork = new OriginationNetwork();
                                    TempOriginationNetwork.Name = i.OriginationNetwork.ToUpper();
                                    ListNewOriginationNetworks.Add(TempOriginationNetwork);
                                }
                            }
                        }

                        Vanrise.Fzero.Bypass.OriginationNetwork.SaveBulk("OriginationNetworks", ListNewOriginationNetworks);
                    }
                    catch
                    {
                    }



                    break;




                case (int)Enums.SourceTypes.RecievesOnly:




                    // Find Difference in GMT
                    int GMTDifferenceRecievesOnly = 0;
                    GMTDifferenceRecievesOnly = Global_GMT - source.GMT;

                    int CounterRecievesOnly = 1;
                    List<RecievedCall> listRecievedCalls = new List<RecievedCall>();
                    foreach (DataRow i in dt.Rows)
                    {
                        RecievedCall gc = new RecievedCall();
                        float DurationInSeconds = 0;
                        if (i.Table.Columns.Contains("Duration In Seconds"))
                        {
                            if (i["Duration In Seconds"].ToString() != string.Empty)
                            {
                                float.TryParse(i["Duration In Seconds"].ToString(), out DurationInSeconds);
                            }
                        }

                        gc.DurationInSeconds = Convert.ToInt32(DurationInSeconds); ;///1111111
                        ///

                        int ClientID = 1; ///ITPC
                        if (i.Table.Columns.Contains("ClientName"))
                        {
                            if (i["ClientName"].ToString().Trim() != string.Empty)
                            {
                                if (ListClients.Where(x => x.Name == i["ClientName"].ToString().Trim()).Count() > 0)
                                {
                                    ClientID = ListClients.Where(x => x.Name == i["ClientName"].ToString().Trim()).FirstOrDefault().ID;
                                }
                            }
                        }

                        gc.ClientID = ClientID;


                        string a_number = string.Empty;
                        if (i.Table.Columns.Contains("a_number"))
                        {
                            a_number = i["a_number"].ToString();
                        }
                        gc.a_number = a_number;


                        string b_number = string.Empty;
                        if (i.Table.Columns.Contains("b_number"))
                        {
                            b_number = i["b_number"].ToString();
                        }
                        gc.b_number = b_number;

                        string CLI = string.Empty;
                        if (i.Table.Columns.Contains("CLI"))
                        {
                            CLI = i["CLI"].ToString();
                        }
                        gc.CLI = CLI;



                        string Type = "SIP";
                        if (i.Table.Columns.Contains("Type"))
                        {
                            Type = i["Type"].ToString();
                        }
                        gc.Type = Type;


                        string Reference = string.Empty;
                        if (i.Table.Columns.Contains("Reference"))
                        {
                            Reference = i["Reference"].ToString();
                        }
                        gc.Reference = Reference;


                        string Carrier = string.Empty;
                        if (i.Table.Columns.Contains("Carrier"))
                        {
                            Carrier = i["Carrier"].ToString().ToUpper();
                        }
                        gc.Carrier = Carrier;


                        string OriginationNetwork = string.Empty;
                        if (i.Table.Columns.Contains("Origination Network"))
                        {
                            OriginationNetwork = i["Origination Network"].ToString().ToUpper();
                        }
                        gc.OriginationNetwork = OriginationNetwork;



                        // Add Area Code to be Able to Know the Mobile Operator Prefix
                        string NumberWithoutAreaCodeCLI = gc.CLI;

                        if (gc.CLI.StartsWith("+"))
                        {
                            NumberWithoutAreaCodeCLI = gc.CLI.Substring(1);
                        }

                        if (gc.CLI.StartsWith("00"))
                        {
                            NumberWithoutAreaCodeCLI = gc.CLI.Substring(2);
                        }

                        else if (gc.CLI.StartsWith("0"))
                        {
                            NumberWithoutAreaCodeCLI = gc.CLI.Substring(1);
                        }




                        foreach (var code in CountryCodes)
                        {
                            if (gc.CLI.StartsWith(code))
                            {
                                NumberWithoutAreaCodeCLI = gc.CLI.Substring(code.Count());
                            }

                            if (gc.CLI.StartsWith("+" + code))
                            {
                                NumberWithoutAreaCodeCLI = gc.CLI.Substring(code.Count() + 1);
                            }

                            if (gc.CLI.StartsWith("00" + code))
                            {
                                NumberWithoutAreaCodeCLI = gc.CLI.Substring(code.Count() + 2);
                            }
                        }

                        
                        
                        foreach (MobileOperator j in lstMobileOperators)
                        {
                            if (j.User.FullName == Global_DefaultMobileOperator)
                            {
                                gc.MobileOperatorID = j.ID;
                            }
                        }


                        foreach (MobileOperator j in lstMobileOperators)
                        {
                            List<string> Prefixes = j.User.Prefix.Split(';').ToList<string>();

                            foreach (string p in Prefixes)
                            {
                                if (p != string.Empty)
                                {
                                    int result = 0;
                                    if (int.TryParse(p, out result))
                                    {
                                        if (NumberWithoutAreaCodeCLI.StartsWith(p))
                                        {
                                            gc.MobileOperatorID = j.ID;
                                        }
                                    }


                                }
                            }
                        }



                        DateTime AttemptDateTime = new DateTime();
                        if (i.Table.Columns.Contains("Attempt Date Time"))
                        {
                            if (i["Attempt Date Time"].ToString() != string.Empty)
                            {
                                DateTime.TryParse(i["Attempt Date Time"].ToString(), out AttemptDateTime);
                                // Apply Difference in GMT
                                AttemptDateTime = AttemptDateTime.AddHours(GMTDifferenceRecievesOnly);
                            }
                        }
                        gc.AttemptDateTime = AttemptDateTime;





                        gc.ImportID = import.ID;
                        gc.SourceID = SourceID;
                        CounterRecievesOnly++;

                        if (AttemptDateTime != DateTime.Parse("1/1/0001 12:00:00 AM"))
                        {
                            listRecievedCalls.Add(gc);

                        }


                    }
                    RecievedCall.SaveBulk("RecievedCalls", listRecievedCalls);


                    try
                    {
                        foreach (var i in listRecievedCalls)
                        {
                            if (ListCarriers.Where(x => x.Name == i.Carrier.ToUpper()).Count() == 0)
                            {
                                if (ListNewCarriers.Where(x => x.Name == i.Carrier.ToUpper()).Count() == 0)
                                {
                                    Carrier TempCarrier = new Carrier();
                                    TempCarrier.Name = i.Carrier.ToUpper();
                                    ListNewCarriers.Add(TempCarrier);
                                }
                            }
                        }

                        Vanrise.Fzero.Bypass.Carrier.SaveBulk("Carriers", ListNewCarriers);


                        foreach (var i in listRecievedCalls)
                        {
                            if (ListOriginationNetworks.Where(x => x.Name == i.OriginationNetwork.ToUpper()).Count() == 0)
                            {
                                if (ListNewOriginationNetworks.Where(x => x.Name == i.OriginationNetwork.ToUpper()).Count() == 0)
                                {
                                    OriginationNetwork TempOriginationNetwork = new OriginationNetwork();
                                    TempOriginationNetwork.Name = i.OriginationNetwork.ToUpper();
                                    ListNewOriginationNetworks.Add(TempOriginationNetwork);
                                }
                            }
                        }

                        Vanrise.Fzero.Bypass.OriginationNetwork.SaveBulk("OriginationNetworks", ListNewOriginationNetworks);
                    }
                    catch
                    {
                    }

                    break;
            }

            CasesLog.SaveBulk("CasesLogs", ListCasesLogs);
        }

        public static void Confirm(string SourceName, DataTable dt, int? ImportedBy)
        {
            Confirm(Source.Load(SourceName).ID, dt, ImportedBy);
        }

        public static DataTable GetDataFromExcel(string filePath, string SourceName)
        {
            return GetDataFromExcel(filePath, Source.Load(SourceName).ID);
        }

        public static DataTable GetDataFromXml(string filePath, string SourceName)
        {
            return GetDataFromXml(filePath, Source.Load(SourceName).ID);
        }

        public static DataTable GetDataFromExcel(string filePath, int SourceId)
        {
            string strConn;
            if (filePath.Substring(filePath.LastIndexOf('.')).ToLower() == ".xlsx")
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"";
            else
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0\"";

            DataSet ds = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();

                DataTable schemaTable = conn.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                foreach (DataRow schemaRow in schemaTable.Rows)
                {
                    string sheet = schemaRow["TABLE_NAME"].ToString();

                    if (!sheet.EndsWith("_"))
                    {
                        try
                        {
                            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                            cmd.CommandType = CommandType.Text;

                            DataTable outputTable = new DataTable(sheet);
                            ds.Tables.Add(outputTable);
                            new OleDbDataAdapter(cmd).Fill(outputTable);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message + string.Format("Sheet:{0}.File:F{1}", sheet, filePath), ex);
                        }
                    }
                }
            }

            DataTable dtxls = ds.Tables[0];
            List<SourceMapping> listSourceMappingxls = SourceMapping.GetSourceMappings(SourceId);

            int colNumberxls = 0;

            foreach (DataColumn dc in dtxls.Columns)
            {
                SourceMapping sourceMapping = listSourceMappingxls.Where(x => x.ColumnName == dtxls.Columns[colNumberxls].ColumnName).FirstOrDefault();
                if (sourceMapping != null)
                {
                    dc.ColumnName = sourceMapping.PredefinedColumn.Name;
                }
                else
                {
                    dc.ColumnName = dc + " : UnMapped";
                }

                colNumberxls++;
            }
            return dtxls;

        }

        public static DataTable GetDataFromXml(string filePath, int SourceId)
        {

            try
            {
                DataSet ds = new DataSet("GeneratedCalls");
                ds.ReadXml(filePath);

                DataTable dtXml = ds.Tables[0];

                List<SourceMapping> listSourceMappingXml = SourceMapping.GetSourceMappings(SourceId);

                int colNumberXml = 0;

                foreach (DataColumn DC in dtXml.Columns)
                {
                    SourceMapping sourceMapping = listSourceMappingXml.Where(x => x.ColumnName == dtXml.Columns[colNumberXml].ColumnName).FirstOrDefault();

                    if (sourceMapping != null)
                    {
                        DC.ColumnName = sourceMapping.PredefinedColumn.Name;
                    }
                    else
                    {
                        DC.ColumnName = DC.ColumnName + " : UnMapped";
                    }

                    colNumberXml++;
                }


                return dtXml;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCalls.GetDataFromXml(" + filePath.ToString() + ")", err);
                return null;
            }


        }

        public static bool SaveBulk(string tableName, List<GeneratedCall> listGeneratedCalls)
        {
            bool success = false;
            try
            {
                Manager.InsertData(listGeneratedCalls.ToList(), tableName, "FMSConnectionString");
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.SaveBulk(" + listGeneratedCalls.Count.ToString() + ")", err);
            }
            return success;
        }

        public static bool Feedback(List<int?> ListIds, int MobileOperatorFeedbackID, string FeedbackNotes, DateTime FeedbackDateTime)
        {

            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    foreach (int id in ListIds)
                    {
                        foreach (GeneratedCall generatedCall in context.GeneratedCalls.Where(x => x.ID == id).ToList())
                        {
                            generatedCall.MobileOperatorFeedbackID = MobileOperatorFeedbackID;
                            generatedCall.FeedbackDateTime = FeedbackDateTime;
                            generatedCall.FeedbackNotes = FeedbackNotes;
                            context.Entry(generatedCall).State = System.Data.EntityState.Modified;
                        }
                    }

                    context.SaveChanges();
                }


                List<CasesLog> ListCasesLogs = new List<CasesLog>();

                foreach (int ID in ListIds)
                {
                    CasesLog cl1 = new CasesLog();
                    cl1.UpdatedOn = DateTime.Now;
                    cl1.ChangeTypeID = (int)Enums.ChangeType.ChangedStatus;
                    cl1.GeneratedCallID = ID;
                    cl1.MobileOperatorFeedbackID = MobileOperatorFeedbackID;
                    ListCasesLogs.Add(cl1);
                }

                CasesLog.SaveBulk("CasesLogs", ListCasesLogs);




                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.UserPermission.Assign(" + ListIds.Count.ToString() + ")", err);
            }
            return success;




        }

        public static bool UpdateReportStatus(List<int> ListIds, int ReportingStatusID, int? ReportingStatusChangedBy)
        {

            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    foreach (int id in ListIds)
                    {
                        foreach (GeneratedCall generatedCall in context.GeneratedCalls.Where(x => x.ID == id).ToList())
                        {
                            generatedCall.ReportingStatusID = ReportingStatusID;
                            generatedCall.ReportingStatusChangedBy = ReportingStatusChangedBy;
                            context.Entry(generatedCall).State = System.Data.EntityState.Modified;
                        }
                    }

                    context.SaveChanges();
                }


                List<CasesLog> ListCasesLogs = new List<CasesLog>();

                foreach (int ID in ListIds)
                {
                    CasesLog cl1 = new CasesLog();
                    cl1.UpdatedOn = DateTime.Now;
                    cl1.ChangeTypeID = (int)Enums.ChangeType.ChangedStatus;
                    cl1.GeneratedCallID = ID;
                    cl1.ReportingStatusID = ReportingStatusID;
                    ListCasesLogs.Add(cl1);
                }

                CasesLog.SaveBulk("CasesLogs", ListCasesLogs);

                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.UserPermission.UpdateReportStatus(" + ListIds.Count.ToString() + ")", err);
            }
            return success;




        }

        public static bool UpdateReportStatusSecurity(List<int> ListIds, int ReportingStatusID, int? ReportingStatusChangedBy)
        {

            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    foreach (int id in ListIds)
                    {
                        foreach (GeneratedCall generatedCall in context.GeneratedCalls.Where(x => x.ID == id).ToList())
                        {
                            generatedCall.ReportingStatusSecurityID = ReportingStatusID;
                            generatedCall.ReportingStatusChangedBy = ReportingStatusChangedBy;
                            context.Entry(generatedCall).State = System.Data.EntityState.Modified;
                        }
                    }

                    context.SaveChanges();
                }


                List<CasesLog> ListCasesLogs = new List<CasesLog>();

                foreach (int ID in ListIds)
                {
                    CasesLog cl1 = new CasesLog();
                    cl1.UpdatedOn = DateTime.Now;
                    cl1.ChangeTypeID = (int)Enums.ChangeType.ChangedStatus;
                    cl1.GeneratedCallID = ID;
                    cl1.ReportingStatusID = ReportingStatusID;
                    ListCasesLogs.Add(cl1);
                }

                CasesLog.SaveBulk("CasesLogs", ListCasesLogs);

                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.UpdateReportStatusSecurity(" + ListIds.Count.ToString() + ")", err);
            }
            return success;




        }

        public static List<ViewGeneratedCall> GetReportedCalls(string ReportID, int DifferenceInGMT)
        {
            List<ViewGeneratedCall> GeneratedCallsList = new List<ViewGeneratedCall>();

            try
            {
                using (Entities context = new Entities())
                {
                    GeneratedCallsList = context.ViewGeneratedCalls
                                          .Where(u => u.ReportRealID == ReportID)
                                            .OrderByDescending(u => u.AttemptDateTime)
                                            .ToList();


                    if (DifferenceInGMT != 0)

                        foreach (ViewGeneratedCall gc in GeneratedCallsList)
                        {
                            gc.AttemptDateTime = gc.AttemptDateTime.AddHours(DifferenceInGMT);
                        }
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetReportedCalls()", err);
            }

            return GeneratedCallsList;
        }

        public static List<ViewGeneratedCall> GetReportedSecCalls(int ReportID, int DifferenceInGMT)
        {
            List<ViewGeneratedCall> GeneratedCallsList = new List<ViewGeneratedCall>();

            try
            {
                using (Entities context = new Entities())
                {
                    GeneratedCallsList = context.ViewGeneratedCalls
                                          .Where(u => u.ReportSecID == ReportID)
                                            .OrderByDescending(u => u.AttemptDateTime)
                                            .ToList();


                    if (DifferenceInGMT != 0)

                        foreach (ViewGeneratedCall gc in GeneratedCallsList)
                        {
                            gc.AttemptDateTime = gc.AttemptDateTime.AddHours(DifferenceInGMT);
                        }
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetReportedCalls()", err);
            }

            return GeneratedCallsList;
        }

        public static List<vwFraudCase> GetFraudCases
        (
            int ClientID,
            int MobileOperatorID,
            DateTime? FromAttemptDateTime,
            DateTime? ToAttemptDateTime,
            int OnnetorOffnet,
            bool IsAdmin,
            int DifferenceInGMT

        )
        {
            List<vwFraudCase> GeneratedCallsList = new List<vwFraudCase>();

            try
            {

                using (Entities context = new Entities())
                {
                    var _ClientID = new SqlParameter("@ClientID", ClientID);
                    var _MobileOperatorID = new SqlParameter("@MobileOperatorID", MobileOperatorID);
                    var _FromAttemptDateTime = new SqlParameter("@StartDate", FromAttemptDateTime);
                    var _ToAttemptDateTime = new SqlParameter("@EndDate", ToAttemptDateTime);
                    var _OnnetorOffnet = new SqlParameter("@OnnetorOffnet", OnnetorOffnet);
                    var _IsAdmin = new SqlParameter("@IsAdmin", IsAdmin);

                    GeneratedCallsList = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwFraudCase>("prGetFraudCases @MobileOperatorID,  @StartDate, @EndDate, @ClientID, @OnnetorOffnet, @IsAdmin", _MobileOperatorID, _FromAttemptDateTime, _ToAttemptDateTime, _ClientID, _OnnetorOffnet, _IsAdmin).ToList();

                    if (DifferenceInGMT != 0)

                        foreach (vwFraudCase gc in GeneratedCallsList)
                        {
                            gc.FirstAttemptDateTime = gc.FirstAttemptDateTime.AddHours(DifferenceInGMT);
                            gc.LastAttemptDateTime = gc.LastAttemptDateTime.AddHours(DifferenceInGMT);
                        }
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases()", err);
            }

            return GeneratedCallsList;
        }


        public static List<vwFraudCase> GetFraudCases_LoadonDemand
      (
          int ClientID,
          int MobileOperatorID,
          DateTime? FromAttemptDateTime,
          DateTime? ToAttemptDateTime,
          int OnnetorOffnet,
          bool IsAdmin,
          int DifferenceInGMT,
          int PageIndex,
          int PageSize

      )
        {
            List<vwFraudCase> GeneratedCallsList = new List<vwFraudCase>();

            try
            {

                using (Entities context = new Entities())
                {
                    var _ClientID = new SqlParameter("@ClientID", ClientID);
                    var _MobileOperatorID = new SqlParameter("@MobileOperatorID", MobileOperatorID);
                    var _FromAttemptDateTime = new SqlParameter("@StartDate", FromAttemptDateTime);
                    var _ToAttemptDateTime = new SqlParameter("@EndDate", ToAttemptDateTime);
                    var _OnnetorOffnet = new SqlParameter("@OnnetorOffnet", OnnetorOffnet);
                    var _IsAdmin = new SqlParameter("@IsAdmin", IsAdmin);
                    var _PageIndex = new SqlParameter("@PageIndex", PageIndex);
                    var _PageSize = new SqlParameter("@PageSize", PageSize);
                    var _OnlyCount = new SqlParameter("@OnlyCount", false);

                    GeneratedCallsList = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwFraudCase>("prGetFraudCases_LoadonDemand @MobileOperatorID,  @StartDate, @EndDate, @ClientID, @OnnetorOffnet, @IsAdmin, @PageIndex,@PageSize,@OnlyCount", _MobileOperatorID, _FromAttemptDateTime, _ToAttemptDateTime, _ClientID, _OnnetorOffnet, _IsAdmin, _PageIndex, _PageSize, _OnlyCount).ToList();

                    if (DifferenceInGMT != 0)

                        foreach (vwFraudCase gc in GeneratedCallsList)
                        {
                            gc.FirstAttemptDateTime = gc.FirstAttemptDateTime.AddHours(DifferenceInGMT);
                            gc.LastAttemptDateTime = gc.LastAttemptDateTime.AddHours(DifferenceInGMT);
                        }
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases()", err);
            }

            return GeneratedCallsList;
        }



        public static List<vwAllCase> GetAllCases_LoadonDemand
      (
          int ClientID,
          int MobileOperatorID,
          DateTime? FromAttemptDateTime,
          DateTime? ToAttemptDateTime,
          Boolean IsAdmin,
          int DifferenceInGMT,
          int PageIndex,
          int PageSize

      )
        {
            List<vwAllCase> GeneratedCallsList = new List<vwAllCase>();

            try
            {

                using (Entities context = new Entities())
                {
                    var _ClientID = new SqlParameter("@ClientID", ClientID);
                    var _MobileOperatorID = new SqlParameter("@MobileOperatorID", MobileOperatorID);
                    var _FromAttemptDateTime = new SqlParameter("@StartDate", FromAttemptDateTime);
                    var _ToAttemptDateTime = new SqlParameter("@EndDate", ToAttemptDateTime);
                    var _IsAdmin = new SqlParameter("@IsAdmin", IsAdmin);
                    var _PageIndex = new SqlParameter("@PageIndex", PageIndex);
                    var _PageSize = new SqlParameter("@PageSize", PageSize);
                    var _OnlyCount = new SqlParameter("@OnlyCount", false);

                    GeneratedCallsList = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwAllCase>("prGetAllCases_LoadonDemand @MobileOperatorID, @StartDate, @EndDate, @ClientID, @IsAdmin, @PageIndex,@PageSize,@OnlyCount", _MobileOperatorID, _FromAttemptDateTime, _ToAttemptDateTime, _ClientID, _IsAdmin, _PageIndex, _PageSize, _OnlyCount).ToList();
                    if (DifferenceInGMT != 0)

                        foreach (vwAllCase gc in GeneratedCallsList)
                        {
                            gc.AttemptDateTime = gc.AttemptDateTime.AddHours(DifferenceInGMT);
                        }

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetAllCases()", err);
            }

            return GeneratedCallsList;
        }


        public static int GetFraudCases_LoadonDemand_Count
    (
        int ClientID,
        int MobileOperatorID,
        DateTime? FromAttemptDateTime,
        DateTime? ToAttemptDateTime,
        int OnnetorOffnet,
        bool IsAdmin,
        int DifferenceInGMT,
        int PageIndex,
        int PageSize

    )
        {
            List<vwCount> CountsList = new List<vwCount>();

            try
            {

                using (Entities context = new Entities())
                {
                    var _ClientID = new SqlParameter("@ClientID", ClientID);
                    var _MobileOperatorID = new SqlParameter("@MobileOperatorID", MobileOperatorID);
                    var _FromAttemptDateTime = new SqlParameter("@StartDate", FromAttemptDateTime);
                    var _ToAttemptDateTime = new SqlParameter("@EndDate", ToAttemptDateTime);
                    var _OnnetorOffnet = new SqlParameter("@OnnetorOffnet", OnnetorOffnet);
                    var _IsAdmin = new SqlParameter("@IsAdmin", IsAdmin);
                    var _PageIndex = new SqlParameter("@PageIndex", PageIndex);
                    var _PageSize = new SqlParameter("@PageSize", PageSize);
                    var _OnlyCount = new SqlParameter("@OnlyCount", true);

                    CountsList = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwCount>("prGetFraudCases_LoadonDemand @MobileOperatorID,  @StartDate, @EndDate, @ClientID, @OnnetorOffnet, @IsAdmin, @PageIndex,@PageSize,@OnlyCount", _MobileOperatorID, _FromAttemptDateTime, _ToAttemptDateTime, _ClientID, _OnnetorOffnet, _IsAdmin, _PageIndex, _PageSize, _OnlyCount).ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases()", err);
            }

            return CountsList.FirstOrDefault().Count;
        }



        public static int GetAllCases_LoadonDemand_Count
     (
         int ClientID,
         int MobileOperatorID,
         DateTime? FromAttemptDateTime,
         DateTime? ToAttemptDateTime,
         Boolean IsAdmin,
         int DifferenceInGMT,
         int PageIndex,
         int PageSize

     )
        {
            List<vwCount> CountsList = new List<vwCount>();

            try
            {

                using (Entities context = new Entities())
                {
                    var _ClientID = new SqlParameter("@ClientID", ClientID);
                    var _MobileOperatorID = new SqlParameter("@MobileOperatorID", MobileOperatorID);
                    var _FromAttemptDateTime = new SqlParameter("@StartDate", FromAttemptDateTime);
                    var _ToAttemptDateTime = new SqlParameter("@EndDate", ToAttemptDateTime);
                    var _IsAdmin = new SqlParameter("@IsAdmin", IsAdmin);
                    var _PageIndex = new SqlParameter("@PageIndex", PageIndex);
                    var _PageSize = new SqlParameter("@PageSize", PageSize);
                    var _OnlyCount = new SqlParameter("@OnlyCount", true);

                    CountsList = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwCount>("prGetAllCases_LoadonDemand @MobileOperatorID, @StartDate, @EndDate, @ClientID, @IsAdmin, @PageIndex,@PageSize,@OnlyCount", _MobileOperatorID, _FromAttemptDateTime, _ToAttemptDateTime, _ClientID, _IsAdmin, _PageIndex, _PageSize, _OnlyCount).ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetAllCases_LoadonDemand()", err);
            }

            return CountsList.FirstOrDefault().Count;
        }


        //public static List<ViewGeneratedCall> GetReportedCalls
        //  (
        //      string CaseID,
        //      string b_number,
        //      string CLI,
        //      DateTime? FromSentDateTime,
        //      DateTime? ToSentDateTime,
        //      string ReportID,
        //      int CLIMobileOperatorID,
        //      int B_NumberMobileOperatorID,
        //      int MobileOperatorFeedbackID,
        //      int RecommendedActionID,
        //      int ClientID,
        //      int DifferenceInGMT
        //  )
        //{
        //    List<ViewGeneratedCall> GeneratedCallsList = new List<ViewGeneratedCall>();

        //    try
        //    {

        //        using (Entities context = new Entities())
        //        {
        //            var _CaseID = new SqlParameter("@CaseID", CaseID);
        //            var _b_number = new SqlParameter("@b_number", b_number);
        //            var _CLI = new SqlParameter("@CLI", CLI);
        //            var _FromSentDateTime = new SqlParameter("@FromSentDateTime", FromSentDateTime);
        //            var _ToSentDateTime = new SqlParameter("@ToSentDateTime", ToSentDateTime);
        //            var _ReportID = new SqlParameter("@ReportID", ReportID);
        //            var _CLIMobileOperatorID = new SqlParameter("@CLIMobileOperatorID", CLIMobileOperatorID);
        //            var _B_NumberMobileOperatorID = new SqlParameter("@B_NumberMobileOperatorID", B_NumberMobileOperatorID);
        //            var _MobileOperatorFeedbackID = new SqlParameter("@MobileOperatorFeedbackID", MobileOperatorFeedbackID);
        //            var _RecommendedActionID = new SqlParameter("@RecommendedActionID", RecommendedActionID);
        //            var _ClientID = new SqlParameter("@ClientID", ClientID);
        //            var _DifferenceInGMT = new SqlParameter("@DifferenceInGMT", DifferenceInGMT);

        //            GeneratedCallsList = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<ViewGeneratedCall>("prGetReportedCalls @CaseID,  @b_number, @CLI, @FromSentDateTime, @ToSentDateTime, @ReportID, @CLIMobileOperatorID, @B_NumberMobileOperatorID, @MobileOperatorFeedbackID, @RecommendedActionID, @ClientID ", _CaseID, _b_number, _CLI, _FromSentDateTime, _ToSentDateTime, _ReportID, _CLIMobileOperatorID, _B_NumberMobileOperatorID, _MobileOperatorFeedbackID, _RecommendedActionID, _ClientID).ToList();
        //        }

        //        if (DifferenceInGMT != 0)

        //            foreach (ViewGeneratedCall vgc in GeneratedCallsList)
        //            {
        //                vgc.AttemptDateTime = vgc.AttemptDateTime.AddHours(DifferenceInGMT);
        //            }
        //    }
        //    catch (Exception err)
        //    {
        //        FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetGeneratedCalls()", err);
        //    }

        //    return GeneratedCallsList;
        //}


        public static List<prGetReportedCalls_Result> GetReportedCalls(
              string CaseID,
              string b_number,
              string CLI,
              DateTime? FromSentDateTime,
              DateTime? ToSentDateTime,
              string ReportID,
              int CLIMobileOperatorID,
              int B_NumberMobileOperatorID,
              int MobileOperatorFeedbackID,
              int RecommendedActionID,
              int ClientID,
              int DifferenceInGMT
          )
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prGetReportedCalls(CaseID, b_number, CLI, FromSentDateTime, ToSentDateTime, ReportID, CLIMobileOperatorID, B_NumberMobileOperatorID, MobileOperatorFeedbackID, RecommendedActionID, ClientID).ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewSourceRecieve.GetReportedCalls()", err);
            }

            return null;
        }


        public static List<GeneratedCall> GetCallsDidNotPassLevelTwo(bool LevelTwoComparisonIsObligatory)
        {
            List<GeneratedCall> GeneratedCallsList = new List<GeneratedCall>();

            try
            {
                using (Entities context = new Entities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 18000;

                    if (LevelTwoComparisonIsObligatory)
                    {
                        GeneratedCallsList = context.GeneratedCalls
                                          .Where(u => u.ID > 0
                                            && u.Level1Comparison == true && u.Level2Comparison == false && (u.StatusID == (int)Enums.Statuses.Pending || u.StatusID == (int)Enums.Statuses.Clean))
                                            .OrderBy(u => u.ID)
                                            .ToList();



                    }
                    else
                    {
                        GeneratedCallsList = context.GeneratedCalls
                                       .Where(u => u.ID > 0
                                         && u.Level1Comparison == true && u.Level2Comparison == false && u.StatusID == (int)Enums.Statuses.Pending)
                                         .OrderBy(u => u.ID)
                                         .ToList();



                    }

                    int FirstID = 0;
                    int LastID = 0;
                    GeneratedCall FirstCall = GeneratedCallsList.FirstOrDefault();
                    GeneratedCall LastCall = GeneratedCallsList.LastOrDefault();
                    if (FirstCall != null && LastCall != null)
                    {
                        try
                        {
                            FirstID = FirstCall.ID;
                            LastID = LastCall.ID;
                            context.Database.ExecuteSqlCommand("Delete from GeneratedCalls where Level1Comparison = 1 and Level2Comparison = 0 and StatusID = 1 and ID>=" + FirstID.ToString() + " and ID<= " + LastID.ToString());
                        }
                        catch (Exception err)
                        {
                            FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetCallsDidNotPassLevelTwo.Delete()", err);
                        }
                    }







                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetCallsDidNotPassLevelTwo()", err);
            }

            return GeneratedCallsList;
        }

        public static void PerformLevelTwoComparison(List<GeneratedCall> GeneratedCallsList)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    string Suspect_Low = SysParameter.Suspect_Low.ToString();
                    string Suspect_Middle_From = SysParameter.Suspect_Middle_From.ToString();
                    string Suspect_Middle_To = SysParameter.Suspect_Middle_To.ToString();
                    string Suspect_High = SysParameter.Suspect_High.ToString();


                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 18000;
                    SaveBulk("GeneratedCalls", GeneratedCallsList);

                    ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreCommand("update generatedcalls set level2comparison=1, leveltwocomparisondatetime=getdate(), StatusID=3 , PriorityID= 3 where DurationInSeconds=" + Suspect_Low + " and ToneFeedbackID=2 and StatusID=1", null);
                    ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreCommand("update generatedcalls set level2comparison=1, leveltwocomparisondatetime=getdate(), StatusID=3 , PriorityID= 2 where DurationInSeconds>=" + Suspect_Middle_From + " and DurationInSeconds<" + Suspect_Middle_To + " and ToneFeedbackID=2 and StatusID=1", null);
                    ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreCommand("update generatedcalls set level2comparison=1, leveltwocomparisondatetime=getdate(), StatusID=3 , PriorityID= 1 where DurationInSeconds>=" + Suspect_High + " and ToneFeedbackID=2 and StatusID=1", null);
                    ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreCommand("update generatedcalls set level2comparison=1, leveltwocomparisondatetime=getdate(), StatusID=4 where ToneFeedbackID=1 and StatusID=1", null);



                }







            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.PerformLevelTwoComparison()", err);
            }

        }

        public static List<ViewGeneratedCall> GetFraudCases(int? ClientID, int RecievedMobileOperatorID)
        {
            List<ViewGeneratedCall> GeneratedCallsList = new List<ViewGeneratedCall>();

            try
            {
                using (Entities context = new Entities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 0;
                    GeneratedCallsList = context.ViewGeneratedCalls.Where(x => x.ReportedBefore == false)
                                .Where(u => u.ID > 0
                                  && (u.ClientID == ClientID)
                                  && (u.StatusID == (int)Enums.Statuses.Fraud)
                                  && (u.ReceivedMobileOperatorID == RecievedMobileOperatorID)
                                  && (u.ReportingStatusID != (int)Enums.ReportingStatuses.Reported)
                                  && (u.ReportingStatusID != (int)Enums.ReportingStatuses.Verified)
                                  && (u.ReportingStatusID != (int)Enums.ReportingStatuses.Reopened)
                                  && (u.ReportingStatusID != (int)Enums.ReportingStatuses.Ignored)
                                  )
                                  .OrderByDescending(u => u.AttemptDateTime)
                                  .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases()", err);
            }

            return GeneratedCallsList;
        }

        public static List<ViewGeneratedCall> GetFraudCasesSecurity(int? ClientID, int RecievedMobileOperatorID)
        {
            List<ViewGeneratedCall> GeneratedCallsList = new List<ViewGeneratedCall>();

            try
            {
                using (Entities context = new Entities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 0;
                    GeneratedCallsList = context.ViewGeneratedCalls.Where(x => x.ReportedBeforeSecurity == false)
                                .Where(u => u.ID > 0
                                  && (u.ClientID == ClientID)
                                  && (u.StatusID == (int)Enums.Statuses.Fraud)
                                  && (u.ReceivedMobileOperatorID == RecievedMobileOperatorID)
                                  && (u.ReportingStatusSecurityID != (int)Enums.ReportingStatuses.Reported)
                                  && (u.ReportingStatusSecurityID != (int)Enums.ReportingStatuses.Verified)
                                  && (u.ReportingStatusSecurityID != (int)Enums.ReportingStatuses.Reopened)
                                  && (u.ReportingStatusSecurityID != (int)Enums.ReportingStatuses.Ignored)
                                  )
                                  .OrderByDescending(u => u.AttemptDateTime)
                                  .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCasesSecurity()", err);
            }

            return GeneratedCallsList;
        }   

        public static List<ViewGeneratedCall> GetClientFraudCases(int ClientID)
        {
            List<ViewGeneratedCall> GeneratedCallsList = new List<ViewGeneratedCall>();

            try
            {
                using (Entities context = new Entities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 18000;
                    GeneratedCallsList = context.ViewGeneratedCalls.Where(x => x.ReportedBefore == false)
                                .Where(u => u.ID > 0
                                  && (u.ClientID == ClientID)
                                  && (u.StatusID == (int)Enums.Statuses.Fraud)
                                  && (u.ReportingStatusID != (int)Enums.ReportingStatuses.Reported) 
                                  && (u.ReportingStatusID != (int)Enums.ReportingStatuses.Verified) 
                                  && (u.ReportingStatusID != (int)Enums.ReportingStatuses.Reopened)
                                  && (u.ReportingStatusID != (int)Enums.ReportingStatuses.Ignored)
                                  )
                                  .OrderByDescending(u => u.AttemptDateTime)
                                  .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases()", err);
            }

            return GeneratedCallsList;
        }

        public static List<ViewGeneratedCall> GetClientFraudCasesSecurity(int ClientID)
        {
            List<ViewGeneratedCall> GeneratedCallsList = new List<ViewGeneratedCall>();

            try
            {
                using (Entities context = new Entities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 18000;
                    GeneratedCallsList = context.ViewGeneratedCalls.Where(x => x.ReportedBeforeSecurity == false)
                                .Where(u => u.ID > 0
                                  && (u.ClientID == ClientID)
                                  && (u.StatusID == (int)Enums.Statuses.Fraud)
                                  && (u.ReportingStatusSecurityID != (int)Enums.ReportingStatuses.Reported) 
                                  && (u.ReportingStatusSecurityID != (int)Enums.ReportingStatuses.Verified) 
                                  && (u.ReportingStatusSecurityID != (int)Enums.ReportingStatuses.Reopened) 
                                  && (u.ReportingStatusSecurityID != (int)Enums.ReportingStatuses.Ignored)
                                  )
                                  .OrderByDescending(u => u.AttemptDateTime)
                                  .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases()", err);
            }

            return GeneratedCallsList;
        }

        public static List<vwRepeatedCase> GetRepeatedCases
       (
           int? ClientID,
           int MobileOperatorID,
           int DifferenceInGMT

       )
        {
            List<vwRepeatedCase> RepeatedCasesList = new List<vwRepeatedCase>();

            try
            {

                using (Entities context = new Entities())
                {
                    var _ClientID = new SqlParameter("@ClientID", ClientID);
                    var _MobileOperatorID = new SqlParameter("@MobileOperatorID", MobileOperatorID);

                    RepeatedCasesList = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwRepeatedCase>("prGetRepeatedCases @MobileOperatorID, @ClientID", _MobileOperatorID, _ClientID).ToList();

                    if (DifferenceInGMT != 0)

                        foreach (vwRepeatedCase gc in RepeatedCasesList)
                        {
                            gc.LastAttemptDateTime = gc.LastAttemptDateTime.AddHours(DifferenceInGMT);
                        }
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.GetRepeatedCases()", err);
            }

            return RepeatedCasesList;
        }

        public static List<ViewSummary> GetViewSummary(int ClientID, int MobileOperatorID, DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime)
        {
            List<ViewSummary> listViewSummary = new List<ViewSummary>();
            try
            {
                using (Entities context = new Entities())
                {
                    var _ClientID = new SqlParameter("@ClientID", ClientID);
                    var _MobileOperatorID = new SqlParameter("@MobileOperatorID", MobileOperatorID);
                    var _FromAttemptDateTime = new SqlParameter("@StartDate", FromAttemptDateTime);
                    var _ToAttemptDateTime = new SqlParameter("@EndDate", ToAttemptDateTime);

                    listViewSummary = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<ViewSummary>("prSummary @MobileOperatorID, @StartDate, @EndDate, @ClientID", _MobileOperatorID, _FromAttemptDateTime, _ToAttemptDateTime, _ClientID).ToList();

                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return listViewSummary;
        }

        public static bool UpdateReportingStatus(int ID, int ReportingStatusID)
        {
            GeneratedCall generatedCall = new GeneratedCall();
            try
            {
                using (Entities context = new Entities())
                {
                    generatedCall = context.GeneratedCalls
                       .Where(u => u.ID == ID)
                       .FirstOrDefault();

                    generatedCall.ReportingStatusID = ReportingStatusID;
                    context.Entry(generatedCall).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.GeneratedCall.Ignore(" + ID.ToString() + ")", err);
                return false;
            }
            return true;
        }
    }
}
