using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.FileMissingChecker
{
    public class SequenceFileDataSourceNaming : FileMissingCheckerSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public override void CheckMissingFiles(ICheckMissingFilesContext context)
        {
            throw new NotImplementedException();
        }
    }
}
