using System;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.FileMissingChecker
{
    public class DateTimeFileMissingChecker : Vanrise.Integration.Entities.FileMissingChecker
    {
        public override Guid ConfigId { get { return new Guid("AF88B648-2FAD-4A7E-8240-564019CF4BC3"); } }

        public override void CheckMissingFiles(ICheckMissingFilesContext context)
        {
            throw new NotImplementedException();
        }
    }
}