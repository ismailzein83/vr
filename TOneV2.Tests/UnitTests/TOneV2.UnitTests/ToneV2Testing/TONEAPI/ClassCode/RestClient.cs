
using System;
using System.IO;
using System.Net;
using System.Text;
public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

namespace TONEAPI.ClassCode
{


  public class RestClient
  {
    public string EndPoint { get; set; }
    public HttpVerb Method { get; set; }
    public string ContentType { get; set; }
    public string PostData { get; set; }
    public string Auth_Token { get; set; }
    public RestClient()
    {
      EndPoint = "";
      Method = HttpVerb.GET;
      ContentType = "text/xml";
      PostData = "";
    }
    public RestClient(string endpoint)
    {
      EndPoint = endpoint;
      Method = HttpVerb.GET;
      ContentType = "text/xml";
      PostData = "";
    }
    public RestClient(string endpoint, HttpVerb method)
    {
      EndPoint = endpoint;
      Method = method;
      ContentType = "text/xml";
      PostData = "";
    }

    public RestClient(string endpoint, HttpVerb method, string postData)
    {
      EndPoint = endpoint;
      Method = method;
      ContentType = "text/xml";
      PostData = postData;
    }
    public RestClient(string endpoint, HttpVerb method, string contenttype, string postData)
    {
        EndPoint = endpoint;
        Method = method;
        ContentType = contenttype;
        PostData = postData;
    }
       public RestClient(string endpoint, HttpVerb method, string contenttype,string Auth_token, string postData)
    {
        EndPoint = endpoint;
        Method = method;
        ContentType = contenttype;
        Auth_Token = Auth_token;
        PostData = postData;
    }
      
    public string MakeRequest()
    {
      return MakeRequest("");
    }

    public string Getresposnse(Uri requesturi, string authheaders)
    {


        if(requesturi == null)
        {
            throw new ArgumentException("requsetURI");

        }
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requesturi);
        request.Method = "GET";
        request.ServicePoint.Expect100Continue = false;
        request.ContentType = "";
        request.Headers["Auth-Token"] = authheaders;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        var responseValue = string.Empty;
        using (var responseStream = response.GetResponseStream())
        {
            if (responseStream != null)
                using (var reader = new StreamReader(responseStream))
                {
                    responseValue = reader.ReadToEnd();
                }
        }
        return responseValue;

    }

    public string Makeresposnse(Uri requesturi, string authheaders)
    {


        if (requesturi == null)
        {
            throw new ArgumentException("requsetURI");

        }
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requesturi);
        request.Method = "POST";
        request.ServicePoint.Expect100Continue = false;
        request.ContentType = "application/json;charset=UTF-8";
       
        request.Headers["Auth-Token"] = authheaders;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        var responseValue = string.Empty;
        using (var responseStream = response.GetResponseStream())
        {
            if (responseStream != null)
                using (var reader = new StreamReader(responseStream))
                {
                    responseValue = reader.ReadToEnd();
                }
        }
        return responseValue;

    }
    public string MakeRequests(string parameters, string authheaders)
    {
      var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

      request.Method = Method.ToString();
      request.ContentLength = 0;
      request.ContentType = ContentType;
      request.Headers["Auth-Token"] = authheaders;

      if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
      {
        var encoding = new UTF8Encoding();
        var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
        request.ContentLength = bytes.Length;
        request.Headers.Add(HttpRequestHeader.Authorization, Auth_Token);
        using (var writeStream = request.GetRequestStream())
        {
          writeStream.Write(bytes, 0, bytes.Length);
        }
      }

      using (var response = (HttpWebResponse)request.GetResponse())
      {
        var responseValue = string.Empty;

        if (response.StatusCode != HttpStatusCode.OK)
        {
          var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
          throw new ApplicationException(message);
        }

        // grab the response
        using (var responseStream = response.GetResponseStream())
        {
          if (responseStream != null)
            using (var reader = new StreamReader(responseStream))
            {
              responseValue = reader.ReadToEnd();
            }
        }

        return responseValue;
      }
    }

    public string MakeRequested(string parameters, string authheaders)
    {
        var request = (HttpWebRequest)WebRequest.Create(EndPoint);

        request.Method = Method.ToString();
        request.ContentLength = 0;
        request.ContentType = ContentType;
        request.Headers["Auth-Token"] = authheaders;

        if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
        {
            var encoding = new UTF8Encoding();
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
            request.ContentLength = bytes.Length;
            request.Headers.Add(HttpRequestHeader.Authorization, Auth_Token);
            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }
        }

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            var responseValue = string.Empty;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                throw new ApplicationException(message);
            }

            // grab the response
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
            }

            return responseValue;
        }
    }



    public string MakeRequest(string parameters)
    {
        var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

        request.Method = Method.ToString();
        request.ContentLength = 0;
        request.ContentType = ContentType;

        if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
        {
            var encoding = new UTF8Encoding();
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
            request.ContentLength = bytes.Length;
            request.Headers.Add(HttpRequestHeader.Authorization, Auth_Token);
            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }
        }

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            var responseValue = string.Empty;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                throw new ApplicationException(message);
            }

            // grab the response
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
            }

            return responseValue;
        }
    }


  } // class

}