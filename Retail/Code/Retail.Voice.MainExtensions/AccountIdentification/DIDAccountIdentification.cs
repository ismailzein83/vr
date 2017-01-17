using Retail.Voice.Entities;
using System;

namespace Retail.Voice.MainExtensions
{
    public class DIDAccountIdentification : AccountIdentification
    {
        public override Guid ConfigId { get { return new Guid("1A73D2E9-1419-4B41-AD2B-6AB04930466B"); } }

        public override void Execute(IAccountIdentificationContext context)
        {
            if (!string.IsNullOrEmpty(context.CallingNumber))
                context.CallingAccountId = 381586;

            //if (!string.IsNullOrEmpty(context.CalledNumber))
            //    context.CalledAccountId = 381587;
        }
    }
}
