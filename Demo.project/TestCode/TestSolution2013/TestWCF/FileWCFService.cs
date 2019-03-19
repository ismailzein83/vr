using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TestWCF
{
    public class FileWCFService : IFileWCFService
    {
        public void SendFile(WCFFileRequest request)
        {
            Console.WriteLine("{0:HH:mm:ss}: file received size {1} KB", DateTime.Now, request.Content.Length / 1024);
            //File.WriteAllBytes(@"C:\Output\" + Guid.NewGuid().ToString(), request.Content);
        }
    }

     [ServiceContract(Namespace = "http://runtime.vanrise.com/IFileWCFService")]
    public interface IFileWCFService
    {
        [OperationContract]
        void SendFile(WCFFileRequest request);
    }

    public class WCFFileRequest
    {
        public byte[] Content { get; set; }
    }
}
