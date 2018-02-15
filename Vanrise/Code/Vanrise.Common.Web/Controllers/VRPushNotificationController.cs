using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using Vanrise.Common.Business;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRPushNotification")]
    public class VRPushNotificationController : BaseAPIController
    {
        [HttpGet]
        [Route("Register")]
        public HttpResponseMessage Register()
        {
            if (HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(ProcessRegistration);
            }

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private async Task ProcessRegistration(AspNetWebSocketContext context)
        {
            WebSocket socket = context.WebSocket;
            socket.ThrowIfNull("socket");
            await VRPushNotificationManager.Current.RegisterSocket(socket);
        }
    }
}