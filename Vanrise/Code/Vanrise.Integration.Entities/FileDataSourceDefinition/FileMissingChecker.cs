using System;
using System.Collections.Generic;

namespace Vanrise.Integration.Entities
{
    public class FileMissingChecker
    {
        public Guid FileMissingCheckerId { get; set; }

        public string Name { get; set; }

        public FileMissingCheckerSettings Settings { get; set; }
    }

    public abstract class FileMissingCheckerSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void CheckMissingFiles(ICheckMissingFilesContext context);
    }

    public interface ICheckMissingFilesContext
    {
        string CurrentFileName { get; }

        string LastRetrievedFileName { get; }

        List<string> MissingFileNames { set; }
    }

    public class CheckMissingFilesContext : ICheckMissingFilesContext
    {
        public string CurrentFileName { get; set; }

        public string LastRetrievedFileName { get; set; }

        public List<string> MissingFileNames { get; set; }
    }
}