using System;
using System.Collections.Generic;
using System.Text;

namespace Vanrise.Web.Base
{
    public class VRWebHostOptions
    {
        public VRWebHostOptions()
        {
            this.VirtualDirectoryOptions = new VRWebHostVirtualDirectoryOptions();
        }

        public VRWebHostVirtualDirectoryOptions VirtualDirectoryOptions { get; private set; }
    }

    public class VRWebVirtualDirectory
    {
        public string VirtualPath { get; set; }

        public string PhysicalPath { get; set; }
    }

    public class VRWebHostVirtualDirectoryOptions
    {

        SortedList<int, VRWebVirtualDirectory> _virtualDirectories = new SortedList<int, VRWebVirtualDirectory>(new VRVirtualDirectoryComparer());

        private class VRVirtualDirectoryComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return -x.CompareTo(y);
            }
        }
        
        public VRWebHostVirtualDirectoryOptions()
        {
            AddVirtualDirectory("/Client", @"C:\TFS\Vanrise\Code\Vanrise.Web\Client");
            IncludeModule("Common", @"C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common");
            IncludeModule("Security", @"C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security");
            IncludeModule("VR_GenericData", @"C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData");
        }

        internal IEnumerable<VRWebVirtualDirectory> GetSortedVirtualDirectories()
        {
            return _virtualDirectories.Values;
        }

        public void IncludeRuntimeModule()
        {
            IncludeModule("Runtime", @"C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime");
        }

        public void IncludeIntegrationModule()
        {
            IncludeModule("Integration", @"C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration");
        }

        public void IncludeBusinessProcessModule()
        {
            IncludeModule("BusinessProcess", @"C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess");
        }

        public void IncludeQueueingModule()
        {
            IncludeModule("Queueing", @"C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing");
        }

        public void IncludeReprocessModule()
        {
            IncludeModule("Reprocess", @"C:\TFS\BusinessProcess\Code\Vanrise.Reprocess.Web\Reprocess");
        }

        public void IncludeModule(string moduleName, string physicalPath)
        {
            AddVirtualDirectory($"/Client/{moduleName}", physicalPath);
        }

        public void AddVirtualDirectory(string virtualPath, string physicalPath)
        {
            _virtualDirectories.Add(virtualPath.Length, new VRWebVirtualDirectory
            {
                VirtualPath = virtualPath,
                PhysicalPath = physicalPath
            });
        }
    }

}
