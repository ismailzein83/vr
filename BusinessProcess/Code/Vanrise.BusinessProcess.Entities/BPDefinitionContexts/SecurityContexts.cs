using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{

    public interface IBPDefinitionGetViewInstanceRequiredPermissionsContext
    {
        BPDefinition BPDefinition { get; }

        BaseProcessInputArgument InputArg { get; }
    }

    public class BPDefinitionGetViewInstanceRequiredPermissionsContext : IBPDefinitionGetViewInstanceRequiredPermissionsContext
    {
        public BPDefinition BPDefinition { get; set; }

        public BaseProcessInputArgument InputArg { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveViewAccessContext
    {
        int UserId { get; }

        BPDefinition BPDefinition { get; }
    }

    public class BPDefinitionDoesUserHaveViewAccessContext : IBPDefinitionDoesUserHaveViewAccessContext
    {
        public int UserId { get; set; }

        public BPDefinition BPDefinition { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveStartAccessContext
    {
        int UserId { get; }

        BPDefinition BPDefinition { get; }
    }

    public class BPDefinitionDoesUserHaveStartAccessContext : IBPDefinitionDoesUserHaveStartAccessContext
    {
        public int UserId { get; set; }

        public BPDefinition BPDefinition { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveScheduleTaskContext
    {
        int UserId { get; }

        BPDefinition BPDefinition { get; }
    }

    public class BPDefinitionDoesUserHaveScheduleTaskContext : IBPDefinitionDoesUserHaveScheduleTaskContext
    {
        public int UserId { get; set; }

        public BPDefinition BPDefinition { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext
    {
        IBPDefinitionDoesUserHaveStartAccessContext DefinitionContext { get; }

        BaseProcessInputArgument InputArg { get; }
    }

    public class BPDefinitionDoesUserHaveStartSpecificInstanceAccessContext : IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext
    {
        public IBPDefinitionDoesUserHaveStartAccessContext DefinitionContext { get; set; }

        public BaseProcessInputArgument InputArg { get; set; }
    }

    public interface IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext
    {
        IBPDefinitionDoesUserHaveScheduleTaskContext DefinitionContext { get; }

        BaseProcessInputArgument InputArg { get; }
    }

    public class BPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext : IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext
    {
        public IBPDefinitionDoesUserHaveScheduleTaskContext DefinitionContext { get; set; }

        public BaseProcessInputArgument InputArg { get; set; }
    }

}
