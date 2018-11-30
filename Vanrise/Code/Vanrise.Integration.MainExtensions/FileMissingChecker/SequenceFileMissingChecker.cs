using System;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.FileMissingChecker
{
    public class SequenceFileMissingChecker : Vanrise.Integration.Entities.FileMissingChecker
    {
        public override Guid ConfigId { get { return new Guid("FA37168F-25B8-44B2-8D27-CA0DD3E3265E"); } }

        public override void CheckMissingFiles(ICheckMissingFilesContext context)
        {
            throw new NotImplementedException();
        }
    }
}