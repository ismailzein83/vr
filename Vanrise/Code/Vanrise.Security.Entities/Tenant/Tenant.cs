using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class Tenant
    {
        public int TenantId { get; set; }

        public string Name { get; set; }

        public int? ParentTenantId { get; set; }

        public TenantSettings Settings { get; set; }
    }

    public class TenantSettings
    {
        public Guid TenantTypeId { get; set; }

        public List<TenantConnectionString> ConnectionStrings { get; set; }

        public List<TenantExtendedSetting> ExtendedSettings { get; set; }
    }

    public class TenantConnectionString
    {
        public string ConnectionStringKey { get; set; }

        public string ConnectionString { get; set; }
    }

    public abstract class TenantExtendedSetting
    {

    }

    public class BPTenantSetting : TenantExtendedSetting
    {
        public List<int> AvailableBPDefinitionIds { get; set; }
    }

    public class GenericDataTenantSetting : TenantExtendedSetting
    {
        public List<int> AvailableBusinessEntityDefinitionIds { get; set; }

        public List<Guid> AvailableDataRecordTypeIds { get; set; }
    }
}
