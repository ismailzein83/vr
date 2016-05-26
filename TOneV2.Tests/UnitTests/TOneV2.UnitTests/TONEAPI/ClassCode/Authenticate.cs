using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class Authenticate
    {
        public string setauthentication(RestClient rs, string url, string data)
        {
            string endPoint = url + @"/api/VR_Sec/Security/Authenticate";


            var client = new RestClient(endpoint: endPoint,
                            method: HttpVerb.POST,
                            contenttype: "application/json;charset=UTF-8",
                            postData: data);
            //try
            //{
            //    var json = client.MakeRequest();
            //    // AuthenticationObject obj = JsonConvert.SerializeObject(obj);
            //    RootObject obj = JsonConvert.DeserializeObject<RootObject>(json);
            //    x = obj.AuthenticationObject.Token.ToString();
            //    TextBox1.Text = x;
            //    TextBox3.Text = "Authentication process success \n";

            //}
            //catch
            //{
            //    TextBox4.Text = "Authentication process \n";
            //}


            authenticateclass lg = new authenticateclass();
            string res = lg.Testauthentication(client, url, data, data, data);

            return res;
        }



    }
}