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
    public class FigureIconManger
    {
        public List<VRFigureIcon> GetFigureIconsInfo()
        {
            var virtualPath = "~/Images/figure-icons";
            string physicalPath = VRWebContext.MapVirtualToPhysicalPath(virtualPath);
            List<VRFigureIcon> fileInfos = new List<VRFigureIcon>();
            if (Directory.Exists(physicalPath))
            {
                DirectoryInfo di = new DirectoryInfo(physicalPath);
                FileInfo[] icons = di.GetFiles(); 

                foreach(var icon in icons)
                {
                    fileInfos.Add(new VRFigureIcon() {
                        Name = icon.Name,
                        IconPath = string.Format("{0}/{1}", "/Images/figure-icons",icon.Name)
                    });
                }
            }
          
            return fileInfos;
        }
    }
}
