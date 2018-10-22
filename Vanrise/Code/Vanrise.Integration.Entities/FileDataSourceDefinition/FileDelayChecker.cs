using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class FileDelayChecker
    {
        public Guid FileDelayCheckerId { get; set; }

        public string Name { get; set; }

        public FileDelayCheckerSettings Settings { get; set; }
    }

    public abstract class FileDelayCheckerSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool IsDelayed(IFileDelayCheckerIsDelayedContext context);
    }

    public interface IFileDelayCheckerIsDelayedContext
    {
        DateTime? LastRetrievedFileTime { get; }
    }

    public class FileDelayCheckerIsDelayedContext : IFileDelayCheckerIsDelayedContext
    {
        public DateTime? LastRetrievedFileTime { get; set; }
    }
}