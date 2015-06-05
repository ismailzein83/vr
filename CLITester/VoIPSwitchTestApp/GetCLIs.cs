using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace VoIPSwitchTestApp
{
    public class GetCLIs
    {
        public static bool locked = false;
        public static int OperatorId = 0;
        private static readonly object _syncRoot = new object();

        public void Start()
        {
            locked = false;
            Thread thread = new Thread(new ThreadStart(GetCLI));
            thread.IsBackground = true;
            thread.Start();
        }

        private void GetCLI()
        {
            string Err = null;

            lock (_syncRoot)
                while (locked != true)
                {
                    locked = true;

                    //Expiry minutes
                    int exptime = 0;
                    int.TryParse("2", out exptime);

                    try
                    {
                        List<TestOperator> testoperators = TestOperatorRepository.GetRequestedTestOperators();
                        for (int i = 0; i < testoperators.Count(); i++)
                        {
                            OperatorId = testoperators[i].Id;
                            MontyCall m = MontyCallRepository.LoadbyTestOperatorId(testoperators[i].Id);
                            if (m != null)
                            {
                                GeneratedCall GenEnd = GeneratedCallRepository.Load(m.CallEntryId.Value);
                                if (GenEnd != null)
                                {
                                    if (GenEnd.StartDate.HasValue)
                                    {
                                        if (exptime != 0 && (DateTime.Now.Second - GenEnd.StartDate.Value.Second) > exptime)
                                        {
                                            Err = "Expired - No Line Availables";
                                            GenEnd.EndDate = DateTime.Now;

                                            testoperators[i].ErrorMessage = Err;
                                            testoperators[i].EndDate = DateTime.Now;
                                            TestOperatorRepository.Save(testoperators[i]);
                                        }
                                    }

                                    //Check the call entries
                                    if (GenEnd.EndDate != null)
                                    {
                                        // Check if the Call is failed // don't check the result from Android Service //480 - 487 removed
                                        if ((GenEnd.ResponseCode == "408" && GenEnd.Status != "4" && GenEnd.Status != "6") ||
                                            (GenEnd.ResponseCode == "404" && GenEnd.Status != "4" && GenEnd.Status != "6") ||
                                            (GenEnd.ResponseCode == "503" && GenEnd.Status != "4" && GenEnd.Status != "6"))
                                        //(GenEnd.ResponseCode == "487" && GenEnd.Status != "4" && GenEnd.Status != "6"))
                                        {
                                            testoperators[i].TestCli = GenEnd.SipAccount.User.CallerId;
                                            if (GenEnd.ResponseCode == "408")
                                                testoperators[i].ErrorMessage = "408 - Request Timeout";
                                            if (GenEnd.ResponseCode == "404")
                                                testoperators[i].ErrorMessage = "404 - Not Found";
                                            if (GenEnd.ResponseCode == "503")
                                                testoperators[i].ErrorMessage = "503 - Service Unavailable";
                                            //if (GenEnd.ResponseCode == "487")
                                            //    testoperators[i].ErrorMessage = "487 - Request Terminated";
                                            testoperators[i].EndDate = DateTime.Now;
                                            testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Failed;
                                            TestOperatorRepository.Save(testoperators[i]);
                                            //h = true;
                                        }
                                        else
                                        {
                                            string myConnectionString;

                                            myConnectionString = "server=192.168.22.23;uid=hadi;" + "pwd=h@d!;database=voipswitch;";
                                            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
                                            conn.ConnectionString = myConnectionString;
                                            conn.Open();
                                            int prefixLength = testoperators[i].CarrierPrefix.Length;
                                            int numberLength = GenEnd.Number.Length;
                                            string number = GenEnd.Number.Substring(prefixLength, numberLength);
                                            string query = "SELECT * FROM voipswitch.callsfailed c where id_client = 2 and ip_number='192.168.22.28' and called_number = '555" + number +
                                                "' and (call_start BETWEEN " + GenEnd.StartDate + " AND " + GenEnd.EndDate + ") order by id_failed_call desc limit 20;";
                                            MySqlCommand cmd = new MySqlCommand(query, conn);

                                            MySqlDataReader reader = cmd.ExecuteReader();
                                            while (reader.Read())
                                            {
                                                Console.WriteLine(reader[0]);
                                            }

                                            //Execute query
                                            //cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }
                                }
                            }

                            if (testoperators[i].CreationDate.HasValue)
                            {
                                if (exptime != 0 && (DateTime.Now.Second - testoperators[i].CreationDate.Value.Second) > exptime)
                                {
                                    Err = "Expired Call";
                                    testoperators[i].EndDate = DateTime.Now;

                                    testoperators[i].ErrorMessage = Err;
                                    testoperators[i].EndDate = DateTime.Now;
                                    TestOperatorRepository.Save(testoperators[i]);
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Logger.LogException(ex);
                        TestOperator t = TestOperatorRepository.Load(OperatorId);
                        if (t.EndDate == null)
                        {
                            t.ErrorMessage = "Failed while Processing";
                            t.EndDate = DateTime.Now;
                            TestOperatorRepository.Save(t);
                        }
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    locked = false;

                    System.Threading.Thread.Sleep(1000);
                }
        }
    }
}
