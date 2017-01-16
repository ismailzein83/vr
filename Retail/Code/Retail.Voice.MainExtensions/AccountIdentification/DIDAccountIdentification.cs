using Retail.Voice.Entities;
using System;

namespace Retail.Voice.MainExtensions
{
    public class DIDAccountIdentification : AccountIdentification
    {
        public override Guid ConfigId { get { return new Guid("1A73D2E9-1419-4B41-AD2B-6AB04930466B"); } }

        public override void Execute(IAccountIdentificationContext context)
        {
            context.CallingAccountId = 1;
            context.CalledAccountId = 2;
        }
    }
}
