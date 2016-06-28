
using System.Collections.Generic;
using Vanrise.Entities;
namespace Vanrise.Common.MainExtensions
{
    public class ExchangeRateTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public string URL { get; set; }

        public string Token { get; set; }

        public List<ConnectionStringSetting> ConnectionStrings { get; set; }
    }
}
