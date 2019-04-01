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
    public class ThemeExtendedManager
    {
        public List<VRThemeExtended> GetThemesExtendedInfo()
        {
            var virtualPath = "~/Client/Themes";
            string physicalPath = VRWebContext.MapVirtualToPhysicalPath(virtualPath);
            List<VRThemeExtended> fileInfos = new List<VRThemeExtended>();
            if (Directory.Exists(physicalPath))
            {
                DirectoryInfo di = new DirectoryInfo(physicalPath);
                FileInfo[] themes = di.GetFiles("*.css",SearchOption.AllDirectories); 



                foreach(var theme in themes)
                {
                    fileInfos.Add(new VRThemeExtended() {
                        Name = GetDisplayThemeFileName(theme.Name),
                        ThemePath = string.Format("/{0}/{1}", MapToVritualPath("Client", theme.Directory.FullName), theme.Name)
                    });
                }
            }
          
            return fileInfos.Where(f => f.Name!="theme" && f.Name!= "extended").ToList();
        }

        string GetDisplayThemeFileName(string name)
        {
            int lastindexOfDash = name.LastIndexOf("-") + 1;
            int lastIndexOfDot = name.LastIndexOf(".");
            int length = lastIndexOfDot - lastindexOfDash;
            string displayThemeFile = name.Substring(lastindexOfDash, length);
            return displayThemeFile;
        }

        string MapToVritualPath(string startDirectory, string physicalPath)
        {
           int indexOfStartDirectoryPath = physicalPath.LastIndexOf(startDirectory);
           string vritualPath = physicalPath.Substring(indexOfStartDirectoryPath).Replace(@"\", "/");
           return vritualPath;
        }
    }
}
