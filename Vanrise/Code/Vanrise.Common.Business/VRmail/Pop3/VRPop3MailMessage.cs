using OpenPop.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRPop3MailMessage : VRReceivedMailMessage
    {
        private OpenPop.Mime.Message _message;
        public VRPop3MailMessage(OpenPop.Mime.Message message)
        {
            _message = message;
        }
        public override VRReceivedMailMessageHeader Header
        {
            get {return new VRPop3MailMessageHeader(_message.Headers); }
        }

        public override List<VRFile> GetAttachments()
        {
            VRFileManager fileManager = new VRFileManager();
            List<VRFile> vrFiles = new List<VRFile>();
            foreach (var attachment in _message.FindAllAttachments())
            {
                string[] nameastab = attachment.FileName.Split('.');
                VRFile file = new VRFile()
                {
                    Content = attachment.Body,
                    Name = attachment.FileName,
                    Extension = nameastab[nameastab.Length - 1],
                    IsTemp = true,
                };
                long id = fileManager.AddFile(file);
                file.FileId = id;
                vrFiles.Add(file);
            }
            return vrFiles;
        }
    }
}