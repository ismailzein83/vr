using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace Vanrise.Web
{
    public class VRLocalizationHttpModules : IHttpModule
    {
        #region Fields

        const string TextResourceRegExPattern = "VRRes\\..*?\\.VREnd";
        static Object s_lockObj = new object();
        static Dictionary<string, FileLocalizationInfo> s_fileLocalizationInfos;
        static string s_localizationInfosFilePath;
        static string s_lastResourcesUpdateInfo;

        #endregion

        #region IHttpModule

       
        public void Dispose()
        {

        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += application_BeginRequest;
        }

        void application_BeginRequest(object sender, System.EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
           // string languageIdAsString = context.Request["vrlangId"];
            Guid? languageId = new VRLocalizationManager().GetCurrentLanguageId();
            if (languageId.HasValue && (context.Request.Url.AbsolutePath.EndsWith(".html") || context.Request.Url.AbsolutePath.EndsWith(".js")))
            {
                VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
                string physicalPath = context.Request.PhysicalPath;
                FileLocalizationInfo localizationInfo = GetFileLocalizationInfo(application, physicalPath);
                if (localizationInfo.LocalizedFileName != null)
                {
                    string localizedFilePath = Path.Combine(application.Server.MapPath("/vr-generated"), languageId.ToString(), localizationInfo.LocalizedFileName);
                    if (!File.Exists(localizedFilePath))
                    {
                        StringBuilder fileContentBuilder = new StringBuilder(File.ReadAllText(physicalPath));
                        var directoryName = Path.GetDirectoryName(localizedFilePath);
                        lock (s_lockObj)
                        {
                            if (!Directory.Exists(directoryName))
                                Directory.CreateDirectory(directoryName);
                        }
                        foreach (var resourceKey in localizationInfo.TextResourceKeys)
                        {

                            fileContentBuilder.Replace(String.Concat("VRRes.", resourceKey, ".VREnd"), vrLocalizationManager.GetTranslatedTextResourceValue(resourceKey, null,languageId.Value));//TODO, replace all resources with valid resources
                        }
                        string fileContent = fileContentBuilder.ToString();
                        lock (s_lockObj)
                        {
                            if (!File.Exists(localizedFilePath))
                                File.WriteAllText(localizedFilePath, fileContent);
                        }
                    }
                    string newURL = string.Format("/vr-generated/{0}/{1}", languageId.ToString(), localizationInfo.LocalizedFileName);
                    context.RewritePath(newURL);
                }

            }
        }

        #endregion

        #region Private Methods

        FileLocalizationInfo GetFileLocalizationInfo(HttpApplication application, string originalFilePath)
        {
            LoadLocalizationInfosIfNotLoaded(application);
            string newLastResourcesUpdateInfo = GetLastResourceUpdateInfo();
            if (newLastResourcesUpdateInfo != s_lastResourcesUpdateInfo)
            {
                lock (s_lockObj)
                {
                    s_fileLocalizationInfos.Clear();
                }
                s_lastResourcesUpdateInfo = newLastResourcesUpdateInfo;
                SaveAllLocalizationInfosToDisk();
            }
            FileLocalizationInfo localizationInfo;
            if (!s_fileLocalizationInfos.TryGetValue(originalFilePath, out localizationInfo))
            {
                var newLocalizationInfo = ParseFileAndGetLocalizationInfo(originalFilePath);
                lock (s_lockObj)
                {
                    if (!s_fileLocalizationInfos.TryGetValue(originalFilePath, out localizationInfo))
                    {
                        localizationInfo = newLocalizationInfo;
                        using (StreamWriter sw = new StreamWriter(s_localizationInfosFilePath, true))
                        {
                            sw.WriteLine(LocalizationInfoToString(originalFilePath, localizationInfo));
                            sw.Close();
                        }
                        s_fileLocalizationInfos.Add(originalFilePath, localizationInfo);
                    }
                }
            }
            else
            {
                var fileInfo = new FileInfo(originalFilePath);
                if (fileInfo.LastWriteTime > localizationInfo.ParsedTime)
                {
                    localizationInfo = ParseFileAndGetLocalizationInfo(originalFilePath);
                    lock (s_lockObj)
                    {
                        s_fileLocalizationInfos[originalFilePath] = localizationInfo;
                    }
                    SaveAllLocalizationInfosToDisk();
                }
            }
            return localizationInfo;
        }

        FileLocalizationInfo ParseFileAndGetLocalizationInfo(string originalFilePath)
        {
            string fileContent = File.ReadAllText(originalFilePath);
            var matches = System.Text.RegularExpressions.Regex.Matches(fileContent, TextResourceRegExPattern);

            List<string> textResourceKeys = null;
            if (matches != null && matches.Count > 0)
            {
                textResourceKeys = new List<string>();
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    textResourceKeys.Add(match.Value.Substring(6, match.Value.Length - 12));
                }
            }
            return new FileLocalizationInfo
            {
                TextResourceKeys = textResourceKeys != null ? textResourceKeys.Distinct().ToArray() : null,
                LocalizedFileName = textResourceKeys != null ? String.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(originalFilePath), Guid.NewGuid(), Path.GetExtension(originalFilePath)) : null,
                ParsedTime = DateTime.Now
            };
        }

        string LocalizationInfoToString(string originalFilePath, FileLocalizationInfo localizationInfo)
        {
            return String.Format("{0},{1},{2},{3}",
                originalFilePath,
                localizationInfo.LocalizedFileName != null ? localizationInfo.LocalizedFileName : "",
                localizationInfo.TextResourceKeys != null ? String.Join("|", localizationInfo.TextResourceKeys) : "",
                DateTime.Now);
        }

        void LoadLocalizationInfosIfNotLoaded(HttpApplication application)
        {
            if (s_fileLocalizationInfos == null)
            {
                lock (s_lockObj)
                {
                    if (s_fileLocalizationInfos == null)
                    {
                        string newLastResourcesUpdateInfo = GetLastResourceUpdateInfo();
                        s_localizationInfosFilePath = application.Server.MapPath("/vr-generated/LocalizationInfos.vr");
                        var directoryName = Path.GetDirectoryName(s_localizationInfosFilePath);
                        if (!Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);
                        if (!File.Exists(s_localizationInfosFilePath))
                            File.Create(s_localizationInfosFilePath).Close();
                        s_fileLocalizationInfos = new Dictionary<string, FileLocalizationInfo>();

                        string[] allLines = File.ReadAllLines(s_localizationInfosFilePath);
                        if (allLines.Length > 0)
                        {
                            s_lastResourcesUpdateInfo = allLines[0];
                            if (s_lastResourcesUpdateInfo == newLastResourcesUpdateInfo)
                            {
                                for (int i = 1; i < allLines.Length; i++)
                                {
                                    string line = allLines[i];
                                    string[] parts = line.Split(',');
                                    if (parts != null && parts.Length == 4)
                                    {
                                        string originalFilePath = parts[0];
                                        string localizedFileName = parts[1];
                                        string textResourceKeysJoined = parts[2];
                                        var localizationInfo = new FileLocalizationInfo
                                        {
                                            LocalizedFileName = localizedFileName != String.Empty ? localizedFileName : null,
                                            TextResourceKeys = textResourceKeysJoined != null ? textResourceKeysJoined.Split('|') : null,
                                            ParsedTime = DateTime.Parse(parts[3])
                                        };
                                        if (!s_fileLocalizationInfos.ContainsKey(originalFilePath))
                                            s_fileLocalizationInfos.Add(originalFilePath, localizationInfo);
                                    }
                                }
                            }
                        }

                        if (s_lastResourcesUpdateInfo != newLastResourcesUpdateInfo)
                        {
                            s_lastResourcesUpdateInfo = newLastResourcesUpdateInfo;
                            SaveAllLocalizationInfosToDisk();
                        }
                        CleanUnUsedLocalizedFiles(application, s_fileLocalizationInfos);

                    }
                }
            }
        }

        private void CleanUnUsedLocalizedFiles(HttpApplication application, Dictionary<string, FileLocalizationInfo> s_fileLocalizationInfos)
        {
            IEnumerable<Guid> allLanguageIds = new VRLocalizationLanguageManager().GetAllLanguagesIds();
            string generatedFilesRootPath = application.Server.MapPath("/vr-generated");
            if (allLanguageIds != null && allLanguageIds.Count() > 0)
            {
                HashSet<string> allLocalizedFileNames = new HashSet<string>(s_fileLocalizationInfos.Values.Where(itm => itm.LocalizedFileName != null).Select(itm => itm.LocalizedFileName.ToLower()));
                foreach (var langId in allLanguageIds)
                {
                    string languageDirectory = Path.Combine(generatedFilesRootPath, langId.ToString());
                    if (Directory.Exists(languageDirectory))
                    {
                        foreach (var filePath in Directory.GetFiles(languageDirectory))
                        {
                            string fileName = Path.GetFileName(filePath).ToLower();
                            if (!allLocalizedFileNames.Contains(fileName))
                                File.Delete(filePath);
                        }
                    }
                }
            }
        }

        void SaveAllLocalizationInfosToDisk()
        {
            lock (s_lockObj)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(s_lastResourcesUpdateInfo.ToString());
                foreach (var localizationInfoEntry in s_fileLocalizationInfos)
                {
                    builder.AppendLine(LocalizationInfoToString(localizationInfoEntry.Key, localizationInfoEntry.Value));
                }
                File.WriteAllText(s_localizationInfosFilePath, builder.ToString());
            }
        }

        string GetLastResourceUpdateInfo()
        {
            return DateTime.Now.Minute.ToString();
        }

        #endregion

        #region Private Classes

        private class FileLocalizationInfo
        {
            public string[] TextResourceKeys { get; set; }

            public string LocalizedFileName { get; set; }

            public DateTime ParsedTime { get; set; }
        }

        #endregion
    }
}