using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Reflection;

namespace TABS.Plugins.Framework
{
    public class VirtualPathProvider : System.Web.Hosting.VirtualPathProvider
    {
        public class VirtualFolder : VirtualDirectory
        {
            public VirtualFolder(string virtualPath)
                : base(virtualPath)
            {
                this.childFiles = new List<VirtualFile>();
                this.childDirectories = new List<VirtualFile>();
            }

            List<VirtualFile> childFiles = new List<VirtualFile>();
            List<VirtualFile> childDirectories = new List<VirtualFile>();

            public override System.Collections.IEnumerable Children
            {
                get { return childFiles; }
            }

            public override System.Collections.IEnumerable Directories
            {
                get { return childDirectories; }
            }

            public override System.Collections.IEnumerable Files
            {
                get { return childFiles; }
            }

            public void Add(VirtualFile file)
            {
                this.childFiles.Add(file);
            }
        }

        public class VirtualFile : System.Web.Hosting.VirtualFile
        {
            public string ResourceName { get; protected set; }
            public string PluginName { get; set; }
            protected byte[] Contents { get; set; }

            public VirtualFile(string pluginID, string resourceName, byte[] contents)
                : base("~/T.One.Modules/" + pluginID + "/" + resourceName)
            {
                ResourceName = resourceName;
                PluginName = pluginID;
                Contents = contents;
            }

            public override System.IO.Stream Open()
            {
                return new System.IO.MemoryStream(Contents);
            }
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (!FileExists(virtualPath))
                return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            else
                return null;
        }

        public Dictionary<string, VirtualFile> Files { get; protected set; }
        public Dictionary<string, VirtualFolder> Directories { get; protected set; }


        public static Regex pageAndModuleFinder = new Regex(".*/T.One.Modules/(?<MODULE>[^/]+)/(?<PAGE>[^/?]+)(/|$|[?])", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VirtualPathProvider));

        public override bool DirectoryExists(string virtualDir)
        {
            if (Directories.ContainsKey(virtualDir))
                return true;
            else
                return Previous.FileExists(virtualDir);
        }

        public VirtualPathProvider()
        {
            log.InfoFormat("Creating Virtual Path Provider");

            this.Files = new Dictionary<string, VirtualFile>();
            this.Directories = new Dictionary<string, VirtualFolder>();
            foreach (IPluginModule plugin in PluginManager.LoadedPluginModules)
            {
                foreach (PluginResource resource in plugin.GetResources().Values)
                {
                    VirtualFile file = new VirtualFile(plugin.ID, resource.ResourceName, resource.Contents);
                    string virtualDirectory = file.VirtualPath.Substring(0, file.VirtualPath.LastIndexOf('/') + 1);
                    VirtualFolder folder = null;
                    if (!Directories.TryGetValue(virtualDirectory, out folder))
                    {
                        folder = new VirtualFolder(virtualDirectory);
                        this.Directories.Add(folder.VirtualPath, folder);
                        log.InfoFormat("Creating Virtual Directory: {0}", folder.VirtualPath);
                    }
                    folder.Add(file);
                    log.InfoFormat("Creating Virtual File: {0}", file.VirtualPath);
                    this.Files.Add(file.VirtualPath, file);
                }
            }
            log.InfoFormat("Created {0} Virtual Directories", Directories.Count);
            log.InfoFormat("Created {0} Virtual Files", Files.Count);
            log.InfoFormat("Created Virtual Path Provider");
        }

        public override bool FileExists(string virtualPath)
        {
            if (Files.ContainsKey(virtualPath))
                return true;
            else
                return Previous.FileExists(virtualPath);
        }

        public override System.Web.Hosting.VirtualFile GetFile(string virtualPath)
        {
            if (Files.ContainsKey(virtualPath))
                return Files[virtualPath];
            else
                return Previous.GetFile(virtualPath);
        }

        public override System.Web.Hosting.VirtualDirectory GetDirectory(string virtualDir)
        {
            if (Directories.ContainsKey(virtualDir))
                return Directories[virtualDir];
            else
                return Previous.GetDirectory(virtualDir);
        }
    }
}
