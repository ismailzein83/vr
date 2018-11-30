using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public abstract class FileDelayChecker
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