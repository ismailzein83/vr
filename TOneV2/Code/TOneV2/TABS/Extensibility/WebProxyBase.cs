using System;

namespace TABS.Extensibility
{
    public abstract class WebProxyBase : System.Web.UI.Page
    {
        public static readonly string RequestHeaderName = "OPERATION";
        public static readonly string ResponseOkMessage = "** OK **\r\n\r\n";
        public static readonly string ResponseErrorMessage = "** ERROR **\r\n\r\n";
        public static readonly string CompressionModeHeaderName = "COMPRESSION-MODE";
        public static readonly string GzipCompressionModeHeaderValue = "GZIP";

        /// <summary>
        /// Gets the required header value
        /// You should implement this property...
        /// </summary>
        public abstract string RequiredHeaderValue { get;}

        protected System.IO.MemoryStream _RequestContentsStream;
        
        public bool IsValidModeToRun { get { return Request.Headers[RequestHeaderName] == RequiredHeaderValue; } }
        public bool IsGzipCompressed { get { return Request.Headers[CompressionModeHeaderName] == GzipCompressionModeHeaderValue; } }

        /// <summary>
        /// Gets the contents of the Request as a memory stream.
        /// (decompressed if sent request was compressed)
        /// </summary>
        public virtual System.IO.MemoryStream RequestContentsStream
        {
            get
            {
                if (_RequestContentsStream == null)
                    _RequestContentsStream = GetMemoryStreamFromRequest();
                return _RequestContentsStream;
            }
        }

        /// <summary>
        /// Runs the proxy, return true on success and false when errors occur.
        /// </summary>
        /// <returns></returns>
        protected abstract bool Run();

        /// <summary>
        /// Get the memory stream from the request.
        /// If the stream is compressed it is decompressed as well.
        /// The returned stream position is at the beginning.
        /// </summary>
        /// <returns>The actual stream (decompressed) of bytes sent by the request</returns>
        protected virtual System.IO.MemoryStream GetMemoryStreamFromRequest()
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(Request.ContentLength);

            byte[] buffer = new byte[1024 * 8];
            int bytesRead = 0;

            while (memoryStream.Length < Request.ContentLength)
                while ((bytesRead = Request.InputStream.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, bytesRead);

            memoryStream.Flush();
            memoryStream.Position = 0;

            System.Array.Clear(buffer, 0, buffer.Length);

            if (IsGzipCompressed)
            {
                System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Decompress);
                System.IO.MemoryStream decompressed = new System.IO.MemoryStream((int)memoryStream.Length);
                int firstByte = gzip.ReadByte();

                if (firstByte > -1)
                {
                    decompressed.WriteByte((byte)firstByte);
                    decompressed.Flush();
                }

                do
                {
                    bytesRead = gzip.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        decompressed.Write(buffer, 0, bytesRead);
                        decompressed.Flush();
                    }
                } while (bytesRead > 0);
                decompressed.Flush();
                memoryStream = decompressed;
            }

            memoryStream.Position = 0;

            return memoryStream;
        }

        protected virtual void ResponseOk(string message)
        {
            Response.Write(ResponseOkMessage);
            Response.Write(message);
            Response.Flush();
        }

        protected virtual void ResponseError(string message)
        {
            Response.Write(ResponseErrorMessage);
            Response.Write(message);
            Response.Flush();
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (IsValidModeToRun)
            {
                Response.Clear();
                Response.Buffer = false;
                Run();
                Response.Flush();
                Response.Close();
                Response.End();
            }
            else
            {
                base.OnPreInit(e);
            }
        }
    }
}