using BPMExtended.Main.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.Common;
using Vanrise.Runtime;

namespace ConsoleApplication1
{
    class Program
    {
        private static LocalDataStoreSlot _Slot = Thread.AllocateNamedDataSlot("contextUserId");

        [ThreadStatic]
        static int? s_threadStats;
        static void Main(string[] args)
        {
            BPMExtended.Main.Business.CustomerRequestTypeManager customerRequestTypeManager = new BPMExtended.Main.Business.CustomerRequestTypeManager();
            var requestTypes = customerRequestTypeManager.GetCustomerRequestTypeInfos(CustomerObjectType.Account, new Guid());
            SOM.Main.BP.Arguments.LineSubscriptionRequest lineRequest = new SOM.Main.BP.Arguments.LineSubscriptionRequest
            {
                PhoneNumber = "3544325435",
                DPId = "eee"
            };
            Guid accountId = Guid.NewGuid();
            BPMExtended.Main.Business.CustomerRequestManager requestManager = new BPMExtended.Main.Business.CustomerRequestManager();
            for (int i = 0; i < 50; i++)
            {
                var output = requestManager.CreateLineSubscriptionRequest(BPMExtended.Main.Entities.CustomerObjectType.Account, accountId, lineRequest);
            }

            List<CustomerRequestHeaderDetail> recentRequests = requestManager.GetRecentRequestHeaders(15, BPMExtended.Main.Entities.CustomerObjectType.Account, accountId, null);
            long seqNumber = recentRequests.Min(itm => itm.SequenceNumber);
            while(recentRequests.Count >= 15)
            {
                //foreach(var request in recentRequests)
                //{
                //    List<CustomerRequestLogDetail> logs = requestManager.GetRequestLogs(request.CustomerRequestId, 2, null);
                //    long logSeqNumber = logs.Min(itm => itm.RequestLogId);
                //    while (logs.Count >= 2)
                //    {
                //        logs = requestManager.GetRequestLogs(request.CustomerRequestId, 2, logSeqNumber);
                //        logSeqNumber = logs.Min(itm => itm.RequestLogId);
                //    }
                //}
                recentRequests = requestManager.GetRecentRequestHeaders(15, BPMExtended.Main.Entities.CustomerObjectType.Account, accountId, seqNumber);
                seqNumber = recentRequests.Min(itm => itm.SequenceNumber);
            }
            //var result = VRSOMClient.Post("api/SOM_Main/SOMRequest/GetFilteredSOMRequests", "{\"Query\":{\"EntityId\":\"Account_112\",\"FromTime\":\"2016-01-01\"},\"SortByColumnName\":\"ProcessInstanceId\",\"FromRow\":0,\"ToRow\":10}");
            //SOMRequestManager somRequestManager = new SOMRequestManager();
            //somRequestManager.CreateRequest(CustomerObjectType.Account, Guid.NewGuid(), "Line Subscription: 3987483534", "{\"$type\":\"SOM.Main.BP.Arguments.LineSubscriptionRequest, SOM.Main.BP.Arguments\",\"PhoneNumber\":\"24354\"}");
            OpenPop.Pop3.Pop3Client pop3 = new OpenPop.Pop3.Pop3Client();
            var mailMsg = pop3.GetMessage(3);
            //mailMsg.FindAllAttachments()[0].

            //DataManager dataManager = new DataManager();
            //dataManager.TestDiffException();
            Console.ReadKey();
            InputForm frm = new InputForm();
            frm.ShowDialog();
            
            var msg = String.Format("fdsf {0} dsfdsf", frm.InputText);

            TestNewRuntimeLocking();
            Console.ReadKey();
            GetTelesSites();
            Console.ReadKey();
            string val1 = "885";
            string val2 = "88435435";

            Console.WriteLine(string.Compare(val1 , val2));
            Console.ReadKey();

            s_threadStats = 5;
            Thread.SetData(_Slot, 6);
            Parallel.For(0, 10, (i) =>
            {
                Thread.SetData(_Slot, i);
                    //s_threadStats = i;
                    Thread.Sleep(2000);
                    Console.WriteLine(Thread.GetData(_Slot));
                    
                });
            Console.ReadKey();
            //TransactionScope scope = new TransactionScope();
            //Transaction.Current
            //PingRunningProcessServiceRequest pingRequest = new PingRunningProcessServiceRequest
            //{
            //    RunningProcessId = 1497
            //};
            //string serializedResponse = null;
            //ServiceClientFactory.CreateTCPServiceClient<IInterRuntimeWCFService>("net.tcp://DEVELOPMENTVLAN:45124/InterRuntimeWCFService",
            //                  (client) =>
            //                  {
            //                      var serializedRequest = Serializer.Serialize(pingRequest);
            //                      serializedResponse = client.ExecuteRequest(serializedRequest);
            //                  });

            while(true)
            {
                DateTime nowTime = DateTime.Now;
                Console.WriteLine("Date HashCode is: {0}", nowTime.GetHashCode());
                Console.WriteLine("Struct HashCode is: {0}", (new DateStruct { Date = nowTime }).GetHashCode());
                Console.WriteLine("Date HashCode is: {0}", nowTime.GetHashCode());
                Console.WriteLine("Struct HashCode is: {0}", (new DateStruct { Date = nowTime }).GetHashCode());
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.ReadKey();
            }

            var now = DateTime.Now.ToString();
            var nn = new DateTime(2010, 2, 3, 3, 3, 3, DateTimeKind.Unspecified);
            var nnoString = Vanrise.Common.Serializer.Serialize(nn);
            var obj = new Object();
            Struct1 str00 = new Struct1 { Object = obj };

            Struct1 str01 = new Struct1 { Object = str00 };
            Struct1 str02 = new Struct1 { Object = str01, Object2 = str00 };

            Struct1 str10 = new Struct1 { Object = obj };
            Struct1 str11 = new Struct1 { Object = str10 };
            Struct1 str12 = new Struct1 { Object = str11, Object2 = str10 };
            Console.WriteLine("{0}", (str02.Equals(str12)));
            Console.ReadKey();
            for (int i = 0; i < 10;i++)
            {
                AddAction(i);
            }
                new HashForm().ShowDialog();
            Parallel.For(0, 2, (i) =>
            {
                Console.WriteLine("Iteration '{0}'", i);
            });
            string serialized = Vanrise.Common.Serializer.Serialize(TimeSpan.FromMinutes(23));
            DateTime attempt = DateTime.Now.AddSeconds(-4);
            DateTime? connect = DateTime.MinValue;
            decimal? totalPDD = null;
            for (int i = 0; i < 7296; i++)
            {
                totalPDD = (totalPDD.HasValue ? totalPDD.Value : 0) + (decimal)connect.Value.Subtract(attempt).TotalSeconds;
            }
            Console.WriteLine(totalPDD);
            Console.ReadKey();
        }

        private static void TestNewRuntimeLocking()
        {
            while (true)
            {
                Parallel.For(0, 5, (i) =>
                {
                    try
                    {
                        Vanrise.Runtime.TransactionLocker.Instance.TryLock("test1", () =>
                        {
                            Console.WriteLine("Locked");
                            Console.ReadKey();
                        });
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
            }
        }

        static ConcurrentDictionary<Object, Object> s_actions = new ConcurrentDictionary<Object, Object>();
        static void AddAction(int i)
        {
            string d = String.Format("gggg");
            Func<int> action = () =>
                {
                    Console.WriteLine("rrr", d);
                    return 4;
                };
            s_actions.TryAdd(action, null);
            Action action2 = () =>
            {
                Console.WriteLine("rrr");
            };
            s_actions.TryAdd(action2, null);
        }

        static void GetTelesSites()
        {
            List<dynamic> response = GetTelesRequest("https://c5-iot2-prov.teles.de", "/SIPManagement/rest/v1/domain/16549/user", null);

            ChangeRoutingGroup(response[1], 40615);
        }


        static List<dynamic> GetTelesRequest(string url, string action, byte[] data)
        {
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-C5-Application", "f0106d37-e0d7-4ff4-9397-fd35e7608233");
                client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AdnIucmVzdC53cy5kZTphZG1pbkB2cg==");

                var responseTask = client.GetAsync(action);
                responseTask.Wait();
                if (responseTask.Exception != null)
                    throw responseTask.Exception;
                if (responseTask.Result.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception();
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var rsltTask = responseTask.Result.Content.ReadAsAsync<List<dynamic>>();
                    rsltTask.Wait();
                    if (rsltTask.Exception != null)
                        throw rsltTask.Exception;
                    return rsltTask.Result;
                }
            }
            return null;
        }

        static void ChangeRoutingGroup(dynamic user, int routingGroupId)
        {
            Dictionary<string, string> headers = new Dictionary<string, string> 
            { { "X-C5-Application", "f0106d37-e0d7-4ff4-9397-fd35e7608233" },
            {"Authorization","Basic YWRtaW5AdnIucmVzdC53cy5kZTphZG1pbkB2cg=="}
            };
            user.routingGroupId = routingGroupId;
            var response = Vanrise.Common.VRWebAPIClient.Put<dynamic, Object>("https://c5-iot2-prov.teles.de", String.Format("/SIPManagement/rest/v1/user/{0}", user.id), user, headers);
        }
    }

    public struct DateStruct
    {
        public DateTime Date { get; set; }
    }
    public struct Struct1
    {
        public Object Object { get; set; }

        public Object Object2 { get; set; }
    }
}
