using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace CoreWebStandardLib
{
    public static class VRWebRazor
    {
        static VRWebRazor()
        {
            Bundles = new Dictionary<string, VRBundle>();
        }

        public static string WebRootPath { get; set; }

        static Dictionary<string, VRBundle> Bundles { get; set; }

        public static void Add(VRBundle bundle)
        {
            Bundles.Add(bundle.Name, bundle);
        }

        public static Microsoft.AspNetCore.Html.HtmlString RenderScripts2(string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"<b>{WebRootPath}</b>");
            builder.Append("<br>");
            foreach (var filePath in Directory.GetFiles(WebRootPath, "*.*", SearchOption.AllDirectories))
            {
                builder.AppendLine(filePath);
                builder.Append("<br>");
            }
            return new Microsoft.AspNetCore.Html.HtmlString($"<span>{builder.ToString()}<br><b>{message}<b/><span>");
        }

        public static Microsoft.AspNetCore.Html.HtmlString RenderScripts(string bundleName)
        {
            StringBuilder builder = new StringBuilder();

            VRBundle bundle;
            if (!Bundles.TryGetValue(bundleName, out bundle))
                throw new NullReferenceException($"bundle '{bundleName}'");
            foreach (var filePath in bundle.FilePaths)
            {
                builder.AppendLine($@"<script src=""{filePath}?v=1.33""></script>");
            }
            return new Microsoft.AspNetCore.Html.HtmlString(builder.ToString());
        }
    }

    public abstract class VRBundle
    {
        public static string WebRootPath { get; set; }

        public VRBundle(string name)
        {
            this.Name = name;
            this.FilePaths = new List<string>();
        }

        public string Name { get; private set; }

        public List<string> FilePaths { get; set; }

        public VRBundle Include(params string[] virtualPaths)
        {
            this.FilePaths.AddRange(virtualPaths);
            return this;
        }

        public VRBundle IncludeDirectory(string directoryVirtualPath, string searchPattern, bool searchSubdirectories)
        {
            string rootFolderPath = Path.Combine(WebRootPath, directoryVirtualPath.Replace("/", @"\").TrimStart('\\'));
            string rootVirtualPath = "";
            if (!Directory.Exists(rootFolderPath))
            {
                foreach(VRVirtualDirectory virtualDirectory in VRVirtualDirectoriesConfig.GetConfig().VirtualDirectories)
                {
                    if(directoryVirtualPath.StartsWith(virtualDirectory.VirtualPath))
                    {
                        rootFolderPath = Path.Combine(virtualDirectory.PhysicalPath, directoryVirtualPath.Replace(virtualDirectory.VirtualPath, "").Replace("/", @"\").TrimStart('\\'));
                        rootVirtualPath = virtualDirectory.VirtualPath;
                        break;
                    }
                }
                if (!Directory.Exists(rootFolderPath))
                    throw new Exception($"directory '{rootFolderPath}' not found");
            }
            
            foreach (var filePath in Directory.GetFiles(rootFolderPath, searchPattern, searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                string fileRelativePath = $"{filePath.Replace(rootFolderPath, rootVirtualPath).Replace(@"\", "/")}";
                this.FilePaths.Add(fileRelativePath);
            }
            return this;
        }
    }

    public class VRScriptBundle : VRBundle
    {
        public VRScriptBundle(string name) : base(name)
        {
        }
    }

}
