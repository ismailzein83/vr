﻿
namespace Vanrise.Common.MainExtensions.ExchangeRateUpdateService
{
    public class ExchangeRateTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public string URL { get; set; }

        public string Token { get; set; }
    }
}
