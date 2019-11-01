using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Business
{
    public class PathManager
    {
        static Guid _definitionId = new Guid("95DCF8AF-2273-4356-81E7-081034CCD75B");

        public long CreatePath(PathInput pathInput)
        {
            return 0;
        }

        public long AddConnectionToPath(PathConnectionInput pathConnectionInput)
        {
            return 0;
        }

        public void SetPathReady(long pathId)
        {

        }
    }
}
