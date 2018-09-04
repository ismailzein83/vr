using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CodeListResolverConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_BE_CodeListResolverConfig";
        public string Editor { get; set; }
    }

    public class ExcludedDestinationsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_BE_ExcludedDestinationsConfig";
        public string Editor { get; set; }
    }

    public class CodeListResolver
    {
        public CodeListResolverSettings Settings { get; set; }
    }

    public abstract class CodeListResolverSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract List<string> GetCodeList(ICodeListResolverContext context);
    }

    public interface ICodeListResolverContext
    {
    }

    public abstract class ExcludedDestinations
    {
        public abstract Guid ConfigId { get; }
        public abstract List<string> GetExcludedCodes(ICodeListExcludedContext context);
    }
    public interface ICodeListExcludedContext
    {
    }
}