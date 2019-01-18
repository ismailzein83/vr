//using Vanrise.Common.Business;

//namespace SOM.ST.Business.ConnectionCallInterceptor
//{
//    public class BSCSOnlineInterceptor : VRHttpConnectionCallInterceptor
//    {
//        public string UserName { get; set; }

//        public string Password { get; set; }

//        public string AuthenticationServiceURI { get; set; }

//        public override void InterceptRequest(IVRHttpConnectionInterceptRequestContext context)
//        {
//            var fullAuthenticationServiceURI = string.Concat(context.Connection.BaseURL, AuthenticationServiceURI);
//        }

//        public override void InterceptResponse(IVRHttpConnectionInterceptResponseContext context)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}