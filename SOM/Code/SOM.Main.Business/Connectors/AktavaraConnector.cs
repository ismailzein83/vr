using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SOM.Main.Business
{
    public class AktavaraConnector
    {
        static JsonSerializerSettings s_Settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };
        public string BaseURL { get; set; }

        public T Get<T>(string actionName)
        {
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri(BaseURL);
               
                var responseTask = client.GetAsync(actionName);
                responseTask.Wait();
                if (responseTask.Exception != null)
                    throw responseTask.Exception;
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var rsltTask = responseTask.Result.Content.ReadAsStringAsync();
                    rsltTask.Wait();
                    if (rsltTask.Exception != null)
                        throw rsltTask.Exception;
                    if (rsltTask.Result == null)
                        return default(T);
                    return JsonConvert.DeserializeObject<T>(rsltTask.Result, s_Settings);
                    
                }
                else
                {
                    throw new Exception(String.Format("Error occured when calling action '{0}' on service '{1}'. Error: {2}", actionName, BaseURL, responseTask.Result.ReasonPhrase));
                }
            }
        }

        public Q Post<T, Q>(string actionName, T request, bool serializeWithFullType = false)
        {
            return Vanrise.Common.VRWebAPIClient.Post<T, Q>(BaseURL, actionName, request, null, serializeWithFullType);
        }

    }

    public class TechnicalReservationDetail
    {
        public string POHNE_NUMBER { get; set; }
        public string PATH_TYPE { get; set; }
        public string SWITCH_NAME { get; set; }
        public string SWITCH_OMC { get; set; }
        public string DEV_ID { get; set; }
        public string MDF_NAME { get; set; }
        public string MDF_VERT { get; set; }
        public string MDF_PORT { get; set; }
        public string CABINET_NAME { get; set; }
        public string PRIMARY_PORT { get; set; }
        public string SECONDARY_PORT { get; set; }
        public string DP_NAME { get; set; }
        public string DP_PORT { get; set; }
        public string DP_PORT_ID { get; set; }
        public string DP_ID { get; set; }
        public string TRANSMITTER_NAME { get; set; }
        public string TRANSMITTER_MODULE { get; set; }
        public string TRANSMITTER_PORT { get; set; }
        public string RECEIVER_NAME { get; set; }
        public string RECEIVER_PORT { get; set; }
        public string EXIST_PSTN { get; set; }
        public string EXIST_ISDN { get; set; }
        public string EXIST_DID { get; set; }
        public string SWITCH_ID { get; set; }
    }

    public class DPPort
    {
        public decimal DP_PORT_ID { get; set; }
        public string DP_PORT_NAME { get; set; }
    }


    public class AktavaraPhoneNumber
    {
        public string OBJECT_ID { get; set; }
        public string PHONE_NUMBER { get; set; }
    }

    public class AktavaraDevice
    {
        public string OBJECT_ID { get; set; }
        public string DEVICE_ID { get; set; }
    }

    public class AktavaraPhoneStatusDetail
    {

        public string PHONE_ID { get; set; }
        public string POHNE_NUMBER { get; set; }
        public string PHONE_STATUS { get; set; }
        public string PATH_TYPE { get; set; }
        public string SWITCH_ID { get; set; }
        public string SWITCH_NAME { get; set; }
        public string SWITCH_OMC { get; set; }
        public string DEV_ID { get; set; }
        public string DEV_NAME { get; set; }
        public string DEV_STATUS { get; set; }
        public string MDF_ID { get; set; }
        public string MDF_NAME { get; set; }
        public string MDF_VERT_ID { get; set; }
        public string MDF_VERT { get; set; }
        public string MDF_PORT_ID { get; set; }
        public string MDF_PORT { get; set; }
        public string MDF_PORT_STATUS { get; set; }
        public string CABINET_ID { get; set; }
        public string CABINET_NAME { get; set; }
        public string PRIMARY_PORT_ID { get; set; }
        public string PRIMARY_PORT { get; set; }
        public string PRIMARY_PORT_STATUS { get; set; }
        public string SECONDARY_PORT_ID { get; set; }
        public string SECONDARY_PORT { get; set; }
        public string SECONDARY_PORT_STATUS { get; set; }
        public string DP_ID { get; set; }
        public string DP_NAME { get; set; }
        public string DP_PORT_ID { get; set; }
        public string DP_PORT { get; set; }
        public string DP_PORT_STATUS { get; set; }
        public string TRANSMITTER_ID { get; set; }
        public string TRANSMITTER_NAME { get; set; }
        public string TRANSMITTER_MODULE_ID { get; set; }
        public string TRANSMITTER_MODULE { get; set; }
        public string TRANSMITTER_PORT_ID { get; set; }
        public string TRANSMITTER_PORT_STATUS { get; set; }
        public string TRANSMITTER_PORT { get; set; }
        public string RECEIVER_ID { get; set; }
        public string RECEIVER_NAME { get; set; }
        public string RECEIVER_PORT_ID { get; set; }
        public string RECEIVER_PORT { get; set; }
        public string RECEIVER_PORT_STATUS { get; set; }
        public string EXIST_PSTN { get; set; }
        public string EXIST_ISDN { get; set; }
        public string EXIST_DID { get; set; }
        public string NUM_FREE_DP_PORTS { get; set; }
        public string NUM_FREE_SECONDARY_PORTS { get; set; }

    }
}
