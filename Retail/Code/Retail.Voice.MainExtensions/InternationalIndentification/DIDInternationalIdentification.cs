using Retail.Voice.Entities;
using System;

namespace Retail.Voice.MainExtensions
{
    public class DIDInternationalIdentification : InternationalIdentification
    {
        public override Guid ConfigId { get { return new Guid("6F57934D-DC86-473E-A8E5-5B24289D2086"); } }

        public override void Execute(IInternationalIdentificationContext context)
        {
            context.IsInternational = true;
        }
    }
}
