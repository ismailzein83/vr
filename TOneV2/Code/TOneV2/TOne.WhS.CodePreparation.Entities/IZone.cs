using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public interface IZone : Vanrise.Entities.IDateEffectiveSettings
    {
        long ZoneId { get; }

        string Name { get; }

        List<AddedCode> AddedCodes { get; }
    }

   
    public class ZonesByName : Dictionary<string, List<IZone>> 
    {

    }
}
