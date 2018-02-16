using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Collections.Specialized;
using System.Threading;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System.Configuration;
using Dean.Edwards;
using System.Reflection;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.HelperTools
{
    public class Common
    {
        public static string ServerIP { get { return ConfigurationManager.AppSettings["ServerIP"]; } }
        public static string ActiveDBs { get { return ConfigurationManager.AppSettings["ActiveDBs"]; } }
        public static string PreventCreateDbScript { get { return ConfigurationManager.AppSettings["PreventCreateDbScript"]; } }
        public static string SqlFilesOutputPath { get { return ConfigurationManager.AppSettings["sqlFilesOutputPath"]; } }
        public static string JavascriptsOutputPath { get { return ConfigurationManager.AppSettings["javascriptsOutputPath"]; } }
        public static string BinPath { get { return ConfigurationManager.AppSettings["binPath"]; } }
        public static string VersionDateFormat { get { return ConfigurationManager.AppSettings["VersionDateFormat"]; } }
        public static string VersionNumber { get { return ConfigurationManager.AppSettings["VersionNumber"]; } }
        public static string SQLUsername { get { return ConfigurationManager.AppSettings["SQLUsername"]; } }
        public static string SQLPassword { get { return ConfigurationManager.AppSettings["SQLPassword"]; } }

        public static List<string> GetDBs(string projectName)
        {
            return ConfigurationManager.AppSettings[projectName].ToString().Split('#').ToList();
        }

        #region CompressJS, GRPJS and GRPJSOverridden
        public static void CompressJSFiles(string currentDateShort, string folder, string javascriptsOutputPath, string projectName)
        {
            ECMAScriptPacker p = new ECMAScriptPacker((ECMAScriptPacker.PackerEncoding)ECMAScriptPacker.PackerEncoding.None, false, false);

            if (string.IsNullOrEmpty(javascriptsOutputPath))
            {
                javascriptsOutputPath = Common.JavascriptsOutputPath;
            }
            if (Directory.Exists(string.Format(javascriptsOutputPath, currentDateShort, folder, projectName)))
            {
                var allFiles = Directory.GetFiles(string.Format(javascriptsOutputPath, currentDateShort, folder, projectName), "*.js", SearchOption.AllDirectories);

                string ExtraFilename = string.Empty;

                Parallel.ForEach(allFiles, file =>
                {
                    CompressFile(file, ExtraFilename, p);
                });
            }
        }

        public static void CompressFile(string file, string ExtraFilename, ECMAScriptPacker p)
        {
            File.WriteAllText(string.Format("{0}\\{1}{2}{3}", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file), ExtraFilename, Path.GetExtension(file)), p.Pack(File.ReadAllText(Path.GetFullPath(file))));
        }

        public static void GroupJSFiles(string currentDateShort, string folder, bool overridden, string javascriptsOutputPath, string projectName)
        {
            if (string.IsNullOrEmpty(javascriptsOutputPath))
            {
                javascriptsOutputPath = Common.JavascriptsOutputPath;
            }
            if (Directory.Exists(string.Format(javascriptsOutputPath, currentDateShort, folder, projectName)))
            {
                var allDirectories = Directory.GetDirectories(string.Format(javascriptsOutputPath, currentDateShort, folder, projectName), "*", SearchOption.TopDirectoryOnly);

                Parallel.ForEach(allDirectories, directory =>
                {
                    var allFiles = Directory.GetFiles(directory, "*.js", SearchOption.AllDirectories);

                    var orgDirectoryName = Path.GetFileName(directory);
                    var directoryName = overridden ? string.Format("{0}{1}", orgDirectoryName, "_Overridden") : orgDirectoryName;

                    //create file if not exist
                    if (!File.Exists(string.Format("{0}\\{1}{2}", directory, directoryName, ".js")))
                    {
                        StringBuilder fileContent = new StringBuilder();

                        //add folder content to created file
                        foreach (var file in allFiles)
                        {
                            fileContent.Append(File.ReadAllText(file));
                            fileContent.Append(";");
                            //rename or remove file
                            File.Delete(file);
                            //File.Move(file, string.Format("{0}{1}", file, "processed"));
                        }

                        File.WriteAllText(string.Format("{0}\\{1}{2}", directory, directoryName, ".js"), fileContent.ToString());
                    }
                });
            }
        }



        #endregion

        #region GRPSQL and GRPSQLOverridden
        public static void GroupSQLPostScriptFiles(string currentDateShort, bool overridden, string sqlFilesOutputPath, string projectName)
        {
            if (string.IsNullOrEmpty(sqlFilesOutputPath))
            {
                sqlFilesOutputPath = Common.SqlFilesOutputPath;
            }

            var allDirectories = Directory.GetDirectories(string.Format(sqlFilesOutputPath, currentDateShort, "", projectName), "*", SearchOption.TopDirectoryOnly);

            foreach (var directory in allDirectories)
            {
                var orgDirectoryName = Path.GetFileName(directory);
                var directoryName = overridden ? string.Format("{0}{1}", orgDirectoryName, "_Overridden") : orgDirectoryName;

                //create file if not exist
                if (!File.Exists(string.Format("{0}\\{1}{2}", directory, directoryName, ".sql")))
                {
                    StringBuilder fileContent = new StringBuilder();

                    string[] allFiles = Directory.GetFiles(directory, "*.sql", SearchOption.AllDirectories); ;
                    Array.Sort(allFiles);

                    if (File.Exists(string.Format("{0}\\{1}{2}", directory, orgDirectoryName, ".txt")))
                    {
                        allFiles = File.ReadAllLines(string.Format("{0}\\{1}{2}", directory, orgDirectoryName, ".txt"));

                        if (orgDirectoryName == "DBsStructure")
                        {
                            for (int i = 0; i < allFiles.Length; i++)
                            {
                                string db = allFiles[i].Replace("#VersionDate#", currentDateShort);
                                allFiles[i] = db;
                            }
                        }
                    }

                    //add folder content to created file
                    foreach (var file in allFiles)
                    {
                        string addDicrectory = string.Format("{0}\\", directory);
                        if (file.Contains("\\"))
                        {
                            addDicrectory = string.Empty;
                        }
                        if (File.Exists(string.Format("{0}{1}", addDicrectory, file)))
                        {
                            fileContent.AppendLine(string.Format("-----------------------FILE {0}-----------------------------------------------", file));
                            fileContent.Append(File.ReadAllText(string.Format("{0}{1}", addDicrectory, file)));
                            fileContent.AppendLine();

                            //rename or remove file
                            File.Delete(string.Format("{0}{1}", addDicrectory, file));
                            //File.Move(file, string.Format("{0}{1}", file, "processed"));
                        }
                    }

                    fileContent = fileContent.Replace("#VersionDate#", DateTime.Now.ToString(VersionDateFormat));
                    fileContent = fileContent.Replace("#VersionNumber#", VersionNumber);

                    File.WriteAllText(string.Format("{0}\\{1}{2}", directory, directoryName, ".sql"), fileContent.ToString());
                    File.Delete(string.Format("{0}\\{1}{2}", directory, orgDirectoryName, ".txt"));
                }
            }
        }

        #endregion

        #region GenerateDBStructure

        public static void GenerateEnumerationsScript(string binPath, string currentDateShort, bool overridden, string sqlFilesOutputPath, string projectName)
        {
            if (string.IsNullOrEmpty(sqlFilesOutputPath))
            {
                sqlFilesOutputPath = Common.SqlFilesOutputPath;
            }

            var assemblies = new List<Assembly>();
            string binFullPath = string.Format(binPath, projectName, currentDateShort);
            var tOneFiles = Directory.GetFiles(binFullPath, "TOne*Entities.dll");
            var vanriseFiles = Directory.GetFiles(binFullPath, "Vanrise*Entities.dll");
            var retailFiles = Directory.GetFiles(binFullPath, "Retail*Entities.dll");
            var mediationFiles = Directory.GetFiles(binFullPath, "Mediation*Entities.dll");
            var sOMFiles = Directory.GetFiles(binFullPath, "SOM*Entities.dll");

            foreach (var file in tOneFiles)
                assemblies.Add(Assembly.LoadFile(file));
            foreach (var file in vanriseFiles)
                assemblies.Add(Assembly.LoadFile(file));
            foreach (var file in retailFiles)
                assemblies.Add(Assembly.LoadFile(file));
            foreach (var file in mediationFiles)
                assemblies.Add(Assembly.LoadFile(file));
            foreach (var file in sOMFiles)
                assemblies.Add(Assembly.LoadFile(file));

            List<Enumeration> allEnumerations = new List<Enumeration>();
            foreach (var assembly in assemblies)
            {
                allEnumerations.AddRange(GetEnumerations(assembly));
            }

            StringBuilder scriptBuilder = new StringBuilder();
            foreach (Enumeration enumeration in allEnumerations)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", enumeration.NameSpace, enumeration.Name, enumeration.Description);
            }
            string script =string.Format(@"--[common].[Enumerations]---------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([NameSpace],[Name],[Description])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([NameSpace],[Name],[Description]))
merge	[common].[Enumerations] as t
using	cte_data as s
on		1=1 and t.[NameSpace] = s.[NameSpace] and t.[Name] = s.[Name]
when matched then
	update set
	[Description] = s.[Description]
when not matched by target then
	insert([NameSpace],[Name],[Description])
	values(s.[NameSpace],s.[Name],s.[Description]);", scriptBuilder.ToString());
            
            File.WriteAllText(string.Format(sqlFilesOutputPath, currentDateShort, "Configuration\\Enumerations.sql", projectName), script.ToString());
        }

        public static List<Enumeration> GetEnumerations(Assembly assembly)
        {
            List<Enumeration> result = new List<Enumeration>();
            if (assembly == null)
                return result;
            try
            {
                var assemblyTypes = assembly.GetTypes();
                foreach (Type type in assemblyTypes)
                {
                        if (type.IsEnum)
                        {
                            if (type.Namespace == null)
                                continue;
                            Enumeration enumeration = new Enumeration();
                            enumeration.NameSpace = type.Namespace;
                            enumeration.Name = type.Name;

                            var enumerationValues = new List<string>();
                            foreach (var enumValue in type.GetEnumValues())
                            {
                                int enumerationValueInteger = Convert.ToInt32(enumValue);
                                string enumerationValueName = enumValue.ToString();
                                enumerationValues.Add(string.Format("{0}:{1}", enumerationValueName, enumerationValueInteger));
                            }
                            enumeration.Description = string.Join(", ", enumerationValues);
                            result.Add(enumeration);
                        }
                    
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("error occured:", ex.Message);
            }
            return result;
        }

        public static void GenerateDBStructure(string currentDate, string currentDateShort, List<string> lstDBs, string sqlFilesOutputPath, string projectName)
        {
            if (lstDBs.Count < 1)
            {
                lstDBs.AddRange(GetDBs(projectName));
            }
            if (string.IsNullOrEmpty(sqlFilesOutputPath))
            {
                sqlFilesOutputPath = Common.SqlFilesOutputPath;
            }

            Parallel.ForEach(lstDBs, item =>
            {
                Console.WriteLine(string.Format("Creating database structure for: {0}", item));
                GenerateSQLDBScript(item, lstDBs[0], string.Format(sqlFilesOutputPath, currentDateShort, "DBsStructure", projectName), currentDateShort, currentDateShort, !string.IsNullOrEmpty(projectName));
                Console.WriteLine(string.Format("Finish creating database structure for: {0}", item));
            });
        }

        public static void GenerateSQLDBScript(string item, string mainItem, string sqlFilesOutputPath, string currentDate, string currentDateShort, bool autoGeneration)
        {
            var sb = new StringBuilder();

            Server srv = new Server(Common.ServerIP);

            ServerConnection conContext = new ServerConnection();
            conContext = srv.ConnectionContext;
            conContext.LoginSecure = false;
            conContext.Login = Common.SQLUsername;
            conContext.Password = Common.SQLPassword;
            Server myServer = new Server(conContext);

            Scripter scripter = new Scripter(myServer);
            Database dbname = myServer.Databases[item];

            StringCollection transferScript = GetTransferScript(dbname);

            ScriptingOptions scriptOptions = new ScriptingOptions();
            scriptOptions.ScriptDrops = true;
            scriptOptions.WithDependencies = false;
            scriptOptions.IncludeHeaders = true;
            scriptOptions.IncludeIfNotExists = true;
            scriptOptions.Indexes = true;
            scriptOptions.NoIdentities = true;
            scriptOptions.NonClusteredIndexes = true;
            scriptOptions.SchemaQualify = true;
            scriptOptions.AllowSystemObjects = true;

            bool dbExist = false;

            foreach (string script in dbname.Script(scriptOptions))
            {
                dbExist = DbExist(script, autoGeneration);
                if (!dbExist)
                {
                    sb.AppendLine(script);
                    sb.AppendLine("GO");
                }
                else
                {
                    sb.AppendLine(string.Format("USE [{0}]", mainItem));
                    sb.AppendLine("GO");
                }
            }

            Urn[] DatabaseURNs = new Urn[] { dbname.Urn };
            StringCollection scriptCollection = scripter.Script(DatabaseURNs);
            foreach (string script in scriptCollection)
            {
                dbExist = DbExist(script, autoGeneration);
                if (script.Contains("CREATE DATABASE") && !dbExist)
                {
                    sb.AppendLine(string.Format("CREATE DATABASE [{0}]", item));
                    sb.AppendLine("GO");

                    sb.AppendLine(string.Format("USE [{0}]", item));
                    sb.AppendLine("GO");
                }
                else
                {
                    if (!dbExist)
                    {
                        sb.AppendLine(script);
                        sb.AppendLine("GO");
                    }
                }
            }

            foreach (string script in transferScript)
            {
                if (!script.Contains("CREATE USER") && !script.Contains("sys.sp_addrolemember"))
                {
                    sb.AppendLine(script);
                    sb.AppendLine("GO");
                }
            }

            sb = sb.Replace(item, string.Format("{0}_{1}", item, currentDateShort));

            if (item != mainItem && !item.Contains(mainItem) && !mainItem.Contains(item))
            {
                sb = sb.Replace(mainItem, string.Format("{0}_{1}", mainItem, currentDateShort));
            }

            if (!Directory.Exists(sqlFilesOutputPath))
            {
                Directory.CreateDirectory(sqlFilesOutputPath);
            }
            File.WriteAllText(string.Format("{0}\\{1}_{2}.sql", sqlFilesOutputPath, item, currentDate), sb.ToString());
        }

        private static bool DbExist(string script, bool autoGeneration)
        {
            bool exist = false;

            if (autoGeneration)
            {
                List<string> lst = Common.GetDBs("PreventCreateDbScript");

                foreach (string db in lst)
                {
                    if (!exist)
                    {
                        exist = script.Contains(db);
                    }
                }
            }

            return exist;
        }

        private static StringCollection GetTransferScript(Database database)
        {

            var transfer = new Transfer(database);

            transfer.CopyAllObjects = true;
            transfer.CopyAllSynonyms = true;
            transfer.CopyData = false;

            transfer.CopyAllRoles = false;

            // additional options
            transfer.Options.WithDependencies = true;
            transfer.Options.DriAll = true;
            transfer.Options.Triggers = true;
            transfer.Options.Indexes = true;
            transfer.Options.SchemaQualifyForeignKeysReferences = true;
            transfer.Options.ExtendedProperties = true;
            transfer.Options.IncludeDatabaseRoleMemberships = true;
            transfer.Options.Permissions = true;
            transfer.PreserveDbo = true;

            // generates script
            return transfer.ScriptTransfer();
        }

        #endregion
    }
}
