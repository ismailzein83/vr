using System;
using System.Collections.Generic;

namespace Vanrise.Entities
{
    public class VRConnectionFilter
    {
        public List<IVRConnectionFilter> Filters { get; set; }

        public List<Guid> ConnectionTypeIds { get; set; }
    }

    public interface IVRConnectionFilter
    {
        bool IsMatched(VRConnection vrConnection);
    }
}