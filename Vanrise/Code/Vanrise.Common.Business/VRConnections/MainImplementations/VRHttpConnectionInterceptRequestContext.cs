using System;
using System.Net.Http;

namespace Vanrise.Common.Business
{
    public abstract class VRHttpConnectionCallInterceptor
    {
        public abstract Guid ConfigId { get; }
        public abstract void InterceptRequest(IVRHttpConnectionInterceptRequestContext context);

        public abstract void InterceptResponse(IVRHttpConnectionInterceptResponseContext context);

    }

    public interface IVRHttpConnectionInterceptRequestContext
    {
        VRHttpConnection Connection { get; }
        HttpClient Client { get; }
        string Body { get; set; }
        HttpRequestMessage HttpRequestMessage { get; }
    }

    public class VRHttpConnectionInterceptRequestContext : IVRHttpConnectionInterceptRequestContext
    {
        public VRHttpConnection Connection { get; set; }
        public HttpClient Client { get; set; }
        public string Body { get; set; }
        public HttpRequestMessage HttpRequestMessage { get; set; }
    }

    public interface IVRHttpConnectionInterceptResponseContext
    {
    }

    public class VRHttpConnectionInterceptResponseContext : IVRHttpConnectionInterceptResponseContext
    {
    }
}
