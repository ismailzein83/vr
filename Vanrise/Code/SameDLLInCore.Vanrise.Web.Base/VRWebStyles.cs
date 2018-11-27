using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Web.Base
{
    public static class VRWebStyles
    {
        public static HtmlString Render(string webBundleName)
        {
            string bundleContent = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject(String.Concat("Render_", webBundleName),
                () =>
                {
                    List<string> filePaths = VRWebContext.GetWebBundlePaths(webBundleName);

                    StringBuilder builder = new StringBuilder();

                    var cacheSettingData = new GeneralSettingsManager().GetCacheSettingData();
                    long version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;

                    foreach (var filePath in filePaths)
                    {
                        builder.AppendLine($@"<link href=""{filePath.TrimStart('~')}?v={version}"" rel=""stylesheet""/>");
                    }
                    return builder.ToString();
                });
            return new HtmlString(bundleContent);
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _settingCacheLastCheck;
            protected override bool ShouldSetCacheExpired()
            {
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<Common.Business.SettingManager.CacheManager>().IsCacheExpired(ref _settingCacheLastCheck);
            }
        }
    }
}
