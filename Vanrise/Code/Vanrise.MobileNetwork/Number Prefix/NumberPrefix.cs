using System;
using Vanrise.Entities;

namespace Vanrise.MobileNetwork.Entities
{
    public class NumberPrefix : ICode, IDateEffectiveSettings
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public int MobileNetworkId { get; set; }
    }
}
