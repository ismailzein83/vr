using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{

    public  class CodeListResolverConfig:ExtensionConfiguration
    {
        public string EXTENSION_TYPE { get { return "WhS_BE_CodeListResolverConfig";} }
        public string Editor { get; set; }
    }


    public class CodeListResolver
    {
        public CodeListResolverSettings Settings { get;set; }
        public ExcludedDestinations ExcludedDestinations { get; set; }
    }

    public abstract class CodeListResolverSettings
    {
        public abstract Guid ConfigId { get;  }

        public abstract List<string> GetCodeList(ICodeListResolverContext context);
    }
    public interface ICodeListResolverContext
    {
    }
    public abstract class ExcludedDestinations
    {
        public abstract Guid ConfigId { get; }
    }
  
   
}
