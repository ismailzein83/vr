using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDevProject
    {
        public Guid VRDevProjectID { get; set; }
        public string Name { get; set; }

        public Guid? AssemblyId { get; set; }

        public string AssemblyName { get; set; }

        public DateTime? AssemblyCompiledTime { get; set; }
        public List<VRDevProjectDependency> ProjectDependencies { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModifiedTime { get; set; }
    }
    public class VRDevProjectDependency
    {
        public Guid DependentProjectId { get; set; }
    }
}
