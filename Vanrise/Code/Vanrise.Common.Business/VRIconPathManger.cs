using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRIconPathManger
    {
  
        public List<VRIconPath> GetVRIconPathsInfo(List<VRIconVirtualPath> paths)
        {
            List<VRIconPath> fileInfos = new List<VRIconPath>();

            foreach(var path in paths)
            {
                string pathValue = Utilities.GetEnumDescription<VRIconVirtualPath>(path);
                string physicalPath = VRWebContext.MapVirtualToPhysicalPath(pathValue);

                if (Directory.Exists(physicalPath))
                {
                    DirectoryInfo di = new DirectoryInfo(physicalPath);
                    FileInfo[] icons = di.GetFiles();

                    foreach (var icon in icons)
                    {
                        fileInfos.Add(new VRIconPath()
                        {
                            Name = icon.Name,
                            IconPath = string.Format("{0}/{1}", pathValue.TrimStart('~'), icon.Name)
                        });
                    }
                }
            }
          

            return fileInfos;
        }
    }
}
