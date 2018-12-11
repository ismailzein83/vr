using System;
using System.Collections.Generic;

namespace Vanrise.Integration.Entities
{
    public abstract class FileMissingChecker
    {
        public abstract Guid ConfigId { get; }

        public abstract void CheckMissingFiles(ICheckMissingFilesContext context);
    }

    public interface ICheckMissingFilesContext
    {
        string LastRetrievedFileName { get; }

        string CurrentFileName { get; }

        List<string> MissingFileNames { set; }
    }

    public class CheckMissingFilesContext : ICheckMissingFilesContext
    {
        public string LastRetrievedFileName { get; set; }

        public string CurrentFileName { get; set; }

        public List<string> MissingFileNames { get; set; }
    }
}