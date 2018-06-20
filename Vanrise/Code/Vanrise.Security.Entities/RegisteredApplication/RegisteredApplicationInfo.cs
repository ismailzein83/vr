using System;
using System.Collections.Generic;

namespace Vanrise.Security.Entities
{
    public class RegisteredApplicationInfo
    {
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
    }

    public class RegisteredApplicationFilter
    {
        public List<IRegisteredApplicationFilter> Filters { get; set; }
    }

    public interface IRegisteredApplicationFilter
    {
        bool IsExcluded(IRegisteredApplicationFilterContext context);
    }

    public interface IRegisteredApplicationFilterContext
    {
        RegisteredApplication RegisteredApplication { get; set; }
    }

    public class RegisteredApplicationFilterContext : IRegisteredApplicationFilterContext
    {
        public RegisteredApplication RegisteredApplication { get; set; }
    }
}