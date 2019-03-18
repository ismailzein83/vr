using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public abstract class VRWebBundle
    {
        public static string WebRootPath { get; set; }

        public VRWebBundle(string name)
        {
            this.Name = name;
            this.FilePaths = new List<string>();
        }

        public string Name { get; private set; }

        public List<string> FilePaths { get; set; }

        public VRWebBundle Include(params string[] virtualPaths)
        {
            foreach (var fileRelativePath in virtualPaths)
            {
                AddRelativeFilePath(fileRelativePath);
            }
            return this;
        }

        public VRWebBundle IncludeDirectory(string directoryVirtualPath, string searchPattern, bool searchSubdirectories)
        {
            string directoryPhysicalPath = VRWebContext.MapVirtualToPhysicalPath(directoryVirtualPath);
            directoryPhysicalPath.ThrowIfNull(directoryPhysicalPath);
            if (!Directory.Exists(directoryPhysicalPath))
                throw new Exception($"directory '{directoryPhysicalPath}' not exists");
            foreach (var filePath in Directory.GetFiles(directoryPhysicalPath, searchPattern, searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                string fileRelativePath = filePath.Replace(directoryPhysicalPath, directoryVirtualPath).Replace(@"\", "/");
                AddRelativeFilePath(fileRelativePath);
            }
            return this;
        }

        private void AddRelativeFilePath(string fileRelativePath)
        {
            if (!this.FilePaths.Contains(fileRelativePath))
                this.FilePaths.Add(fileRelativePath);
        }
    }
}
