using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Vanrise.Web.App_Start
{
    public static class Utils
    {
        public static HttpResponseMessage CreateResponseMessage(HttpStatusCode statusCode, string message)
        {
            HttpResponseMessage msg = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            msg.Content = new System.Net.Http.StringContent(message);

            return msg;
        }
    }
}