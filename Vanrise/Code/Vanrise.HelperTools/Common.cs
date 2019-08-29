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
using Vanrise.DevTools.Business;
using Vanrise.DevTools.Entities;

namespace Vanrise.HelperTools
{
    public class Common
    {
        public static string ServerIP { get { return ConfigurationManager.AppSettings["ServerIP"]; } }
        public static string ActiveDBs { get { return ConfigurationManager.AppSettings["ActiveDBs"]; } }
        public static string PreventCreateDbScript { get { return ConfigurationManager.AppSettings["PreventCreateDbScript"]; } }
        public static string PreventAddingStandardConfigurationStructure { get { return ConfigurationManager.AppSettings["PreventAddingStandardConfigurationStructure"]; } }
        public static string StandardStructureLocalDBs { get { return ConfigurationManager.AppSettings["StandardStructureLocalDBs"]; } }
        public static string StandardQueueingStructureLocalDBs { get { return ConfigurationManager.AppSettings["StandardQueueingStructureLocalDBs"]; } }
        public static string CustomStandardStructure { get { return ConfigurationManager.AppSettings["CustomStandardStructure"]; } }
        public static string SqlFilesOutputPath { get { return ConfigurationManager.AppSettings["sqlFilesOutputPath"]; } }
        public static string JavascriptsOutputPath { get { return ConfigurationManager.AppSettings["javascriptsOutputPath"]; } }
        public static string BinPath { get { return ConfigurationManager.AppSettings["binPath"]; } }
        public static string VersionDateFormat { get { return ConfigurationManager.AppSettings["VersionDateFormat"]; } }
        public static string VersionNumber { get { return ConfigurationManager.AppSettings["VersionNumber"]; } }
        public static string SQLUsername { get { return ConfigurationManager.AppSettings["SQLUsername"]; } }
        public static string SQLPassword { get { return ConfigurationManager.AppSettings["SQLPassword"]; } }
        public static string CheckPathLengthOutputPath { get { return ConfigurationManager.AppSettings["checkPathLengthOutputPath"]; } }
        public static string CheckPathLengthSourcePath { get { return ConfigurationManager.AppSettings["checkPathLengthSourcePath"]; } }
        public static int MaxPathLength { get { return int.Parse(ConfigurationManager.AppSettings["maxPathLength"]); } }
        public static List<string> GetDBs(string projectName)
        {
            List<string> lst = new List<string>();
            var obj = ConfigurationManager.AppSettings[projectName];

            if (obj != null)
            {
                //adding common databases
                var defaultObj = ConfigurationManager.AppSettings["StandardConfigurationStructure"];

                bool projectKey = !ItemExistExactMatch(PreventAddingStandardConfigurationStructure, projectName, true);

                if (!string.IsNullOrEmpty(obj.ToString()))
                {
                    lst.AddRange(obj.ToString().Split('#').ToList());
                    if (projectKey)
                    {
                        lst.InsertRange(1, defaultObj.ToString().Split('#'));
                    }
                }
                else
                {
                    if (projectKey)
                    {
                        lst.AddRange(defaultObj.ToString().Split('#').ToList());
                    }
                }
            }
            return lst;
        }

        public static List<string> GetDBs_Schemas(string projectName, string dbName)
        {
            List<string> lst = new List<string>();
            var obj = ConfigurationManager.AppSettings[projectName + "_" + dbName + "_Schemas"];
            if (obj != null)
            {
                //adding default schema
                var defaultObj = ConfigurationManager.AppSettings[dbName + "_Schemas"];
                if (defaultObj != null)
                {
                    lst.AddRange(defaultObj.ToString().Split('#').ToList());
                }
                if (!string.IsNullOrEmpty(obj.ToString()))
                {
                    lst.AddRange(obj.ToString().Split('#').ToList());
                }
            }
            return lst;
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
                            fileContent.AppendLine(File.ReadAllText(file));
                            fileContent.Append(";");
                            fileContent.AppendLine();
                            //rename or remove file
                            File.Delete(file);
                            //File.Move(file, string.Format("{0}{1}", file, "processed"));
                        }

                        File.WriteAllText(string.Format("{0}\\{1}{2}", directory, directoryName, ".js"), fileContent.ToString());
                    }
                });
            }
        }

        public static void CheckPathLength(string currentDateShort, string sourcePath, string outputPath, string projectName, int maxLength)
        {
            if (maxLength == 0)
            {
                maxLength = Common.MaxPathLength;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Common.CheckPathLengthOutputPath;
            }

            if (string.IsNullOrEmpty(sourcePath))
            {
                sourcePath = Common.CheckPathLengthSourcePath;

                if (string.IsNullOrEmpty(projectName))
                {
                    projectName = "Vanrise";
                }
                if (projectName == "TOneV2")
                {
                    sourcePath = sourcePath + "\\TOneV2";
                }
            }

            var allDirectories = Directory.GetDirectories(string.Format(sourcePath, projectName), "Directives", SearchOption.AllDirectories);

            Parallel.ForEach(allDirectories, directory =>
            {
                if (!directory.Contains("Libraries"))
                {
                    var allFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

                    StringBuilder fileContent = new StringBuilder();
                    //fileContent.AppendLine(string.Format("Files exceeded the limit of {0} characters for the directory and file name.", maxLength));

                    //add file path to created file that matched the criteria
                    foreach (var file in allFiles)
                    {
                        //File.WriteAllText(string.Format("{0}\\{1}{2}{3}", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file), ExtraFilename, Path.GetExtension(file)), p.Pack(File.ReadAllText(Path.GetFullPath(file))));
                        var fileLength = file.ToString().Length;

                        if (fileLength > maxLength)
                        {
                            //fileContent.AppendLine("Files exceeded the limit of 248 characters for the directory or 259 for the directory and file name.");
                            fileContent.AppendLine(file.ToString());
                            //fileContent.AppendLine();
                        }
                    }
                    if (fileContent.Length > 0)
                    {
                        Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}", outputPath, currentDateShort, projectName));
                        var directoryName = Path.GetFileName(Directory.GetParent(directory).ToString());
                        File.WriteAllText(string.Format("{0}\\{1}\\{2}\\{3}{4}", outputPath, currentDateShort, projectName, directoryName, ".txt"), fileContent.ToString());
                    }
                }
            });
        }

        public static void GroupCheckPathLengthFiles(string currentDateShort, string filesOutputPath)
        {
            if (string.IsNullOrEmpty(filesOutputPath))
            {
                filesOutputPath = string.Format("{0}\\{1}", Common.CheckPathLengthOutputPath, currentDateShort);
            }
            if (Directory.Exists(filesOutputPath))
            {
                var allDirectories = Directory.GetDirectories(filesOutputPath, "*", SearchOption.TopDirectoryOnly);

                Parallel.ForEach(allDirectories, directory =>
                {
                    var directoryName = Path.GetFileName(directory);

                    StringBuilder fileContent = new StringBuilder();

                    string[] allFiles = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);

                    //add folder content to created file
                    foreach (var file in allFiles)
                    {
                        if (File.Exists(file))
                        {
                            fileContent.AppendLine(File.ReadAllText(file));
                            //rename or remove file
                            File.Delete(file);
                        }
                    }

                    File.WriteAllText(string.Format("{0}\\{1}{2}", directory, directoryName, ".txt"), fileContent.ToString());
                });
            }
        }

        #endregion

        #region GRPSQL and GRPSQLOverridden
        public static void GroupSQLPostScriptFiles(string currentDateShort, bool overridden, string sqlFilesOutputPath, string projectName, string productDisplayName)
        {
            if (string.IsNullOrEmpty(sqlFilesOutputPath))
            {
                sqlFilesOutputPath = Common.SqlFilesOutputPath;
            }

            var allDirectories = Directory.GetDirectories(string.Format(sqlFilesOutputPath, currentDateShort, "", projectName), "*", SearchOption.TopDirectoryOnly);

            productDisplayName = string.IsNullOrEmpty(productDisplayName) ? projectName : productDisplayName;
            foreach (var directory in allDirectories)
            {
                var orgDirectoryName = Path.GetFileName(directory);
                var directoryName = overridden ? string.Format("{0}{1}", orgDirectoryName, "_Overridden_"+ projectName) : orgDirectoryName;

                //create file if not exist in order to create root output file
                if (!File.Exists(string.Format("{0}\\{1}{2}", directory, directoryName, ".sql")))
                {
                    string[] allFiles = Directory.GetFiles(directory, "**.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".sql") || s.EndsWith(".json")).ToArray();

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

                    StringBuilder fileContent = new StringBuilder();
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
                            //convert .json to .sql then do append content
                            var content = File.ReadAllText(string.Format("{0}{1}", addDicrectory, file));
                            if (Path.GetExtension(string.Format("{0}{1}", addDicrectory, file)) == ".json")
                            {
                                var tables = Serializer.Deserialize<GeneratedScriptItemTables>(content);
                                Vanrise.DevTools.Business.VRGeneratedScriptColumnsManager scriptManager = new DevTools.Business.VRGeneratedScriptColumnsManager();
                                content = scriptManager.GenerateQueries(new DevTools.Entities.GeneratedScriptItem { Type = DevTools.Entities.GeneratedScriptType.SQL, Tables = tables });
                            }

                            fileContent.AppendLine(string.Format("-----------------------FILE {0}-----------------------------------------------", file));
                            fileContent.Append(content);
                            fileContent.AppendLine();

                            //rename or remove file
                            File.Delete(string.Format("{0}{1}", addDicrectory, file));
                            //File.Move(file, string.Format("{0}{1}", file, "processed"));
                        }
                    }

                    fileContent = fileContent.Replace("#VersionDate#", DateTime.Now.ToString(VersionDateFormat));
                    fileContent = fileContent.Replace("#VersionNumber#", VersionNumber);
                    fileContent = fileContent.Replace("#ProductName#", productDisplayName);

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
            var inspktFiles = Directory.GetFiles(binFullPath, "RecordAnalysis*Entities.dll");
            var testCallAnalysisFiles = Directory.GetFiles(binFullPath, "TestCallAnalysis*Entities.dll");



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
            foreach (var file in inspktFiles)
                assemblies.Add(Assembly.LoadFile(file));
            foreach (var file in testCallAnalysisFiles)
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
            string script = string.Format(@"--[common].[Enumerations]---------------------------------------------------------------------------
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
                GenerateSQLDBScript(item, lstDBs[0], string.Format(sqlFilesOutputPath, currentDateShort, "DBsStructure", projectName), currentDateShort, currentDateShort, !string.IsNullOrEmpty(projectName), projectName);
                Console.WriteLine(string.Format("Finish creating database structure for: {0}", item));
            });
        }

        public static void GenerateSQLDBScript(string item, string mainItem, string sqlFilesOutputPath, string currentDate, string currentDateShort, bool autoGeneration, string projectName)
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
                dbExist = ItemExist(PreventCreateDbScript, script, autoGeneration);
                if (!dbExist || projectName.Contains("Component-"))
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
                dbExist = ItemExist(PreventCreateDbScript, script, autoGeneration);
                if ((script.Contains("CREATE DATABASE") && !dbExist)
                    || (script.Contains("CREATE DATABASE") && projectName.Contains("Component-"))
                    || (script.Contains("CREATE DATABASE") && projectName.Contains("Component-"))
                    || (script.Contains("CREATE DATABASE") && projectName.Contains("Component-"))
                    )
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

            List<string> lst = Common.GetDBs_Schemas(projectName, item);
            if (lst.Count > 0)
            {
                foreach (string schema in lst)
                {
                    foreach (string script in transferScript)
                    {
                        if (!script.Contains("CREATE USER") && !script.Contains("sys.sp_addrolemember"))
                        {
                            if (script.Contains(schema) && !sb.ToString().Contains(script))
                            {
                                sb.AppendLine(script);
                                sb.AppendLine("GO");
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (string script in transferScript)
                {
                    if (!script.Contains("CREATE USER") && !script.Contains("sys.sp_addrolemember"))
                    {
                        sb.AppendLine(script);
                        sb.AppendLine("GO");
                    }
                }
            }

            sb = sb.Replace(item, string.Format("{0}_{1}", currentDateShort, item));

            if (item != mainItem && !item.Contains(mainItem) && !mainItem.Contains(item))
            {
                sb = sb.Replace(mainItem, string.Format("{0}_{1}", currentDateShort, mainItem));
            }

            //replace main DB retail with related project 
            if (ItemExistExactMatch(CustomStandardStructure, projectName, true))
            {
                sb = sb.Replace(mainItem, "Standard" + projectName + "Structure");
            }

            if (!Directory.Exists(sqlFilesOutputPath))
            {
                Directory.CreateDirectory(sqlFilesOutputPath);
            }
            File.WriteAllText(string.Format("{0}\\{1}_{2}.sql", sqlFilesOutputPath, currentDate, item), sb.ToString());
        }

        private static bool ItemExist(string stringItems, string script, bool autoGeneration)
        {
            bool exist = false;

            if (autoGeneration)
            {
                List<string> lst = stringItems.ToString().Split('#').ToList();

                foreach (string db in lst)
                {
                    if (!exist)
                    {
                        exist = script.Contains(db);
                    }
                    if (exist)
                    {
                        return exist;
                    }
                }
            }

            return exist;
        }

        private static bool ItemExistExactMatch(string stringItems, string script, bool autoGeneration)
        {
            bool exist = false;

            if (autoGeneration)
            {
                List<string> lst = stringItems.ToString().Split('#').ToList();

                foreach (string db in lst)
                {
                    if (!exist)
                    {
                        exist = script.Equals(db);
                    }
                    if (exist)
                    {
                        return exist;
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

        private static StringBuilder GenerateSQLDBScript(string item)
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

            foreach (string script in dbname.Script(scriptOptions))
            {
                sb.AppendLine(script);
                sb.AppendLine("GO");
            }

            Urn[] DatabaseURNs = new Urn[] { dbname.Urn };
            StringCollection scriptCollection = scripter.Script(DatabaseURNs);
            foreach (string script in scriptCollection)
            {
                if (script.Contains("CREATE DATABASE"))
                {
                    sb.AppendLine(string.Format("CREATE DATABASE [{0}]", item));
                    sb.AppendLine("GO");

                    sb.AppendLine(string.Format("USE [{0}]", item));
                    sb.AppendLine("GO");
                }
                else
                {
                    sb.AppendLine(script);
                    sb.AppendLine("GO");
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

            return sb;
        }

        public static void GenerateLocalDB()
        {
            List<string> lstLoggingTransaction = StandardStructureLocalDBs.ToString().Split('#').ToList();
            List<string> lstQueueing = StandardQueueingStructureLocalDBs.ToString().Split('#').ToList();

            string sqlFilesOutputPath = Common.SqlFilesOutputPath;
            string currentDateShort = DateTime.Now.ToString("yyyMMdd");
            sqlFilesOutputPath = string.Format(sqlFilesOutputPath, currentDateShort, "DBsStructure", "LocalDatabases");

            if (!Directory.Exists(sqlFilesOutputPath))
            {
                Directory.CreateDirectory(sqlFilesOutputPath);
            }

            StringBuilder sbLogging = GenerateSQLDBScript("StandardLoggingStructure");            
            StringBuilder sbTransaction = GenerateSQLDBScript("StandardTransactionStructure");
            StringBuilder sbQueueing = GenerateSQLDBScript("StandardQueueingStructure");

            var postscriptLogging = File.ReadAllText(@"C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql");
            var postscriptTransaction = File.ReadAllText(@"C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\BusinessProcess.PostDeployment.sql");
            var postscriptQueueing = File.ReadAllText(@"C:\TFS\TOneV2\Code\TOneV2\SQL.TOneQueueing\Script.PostDeployment.sql");

            foreach (string project in lstLoggingTransaction)
            {
                StringBuilder stProject = new StringBuilder();
                
                stProject.Append(sbLogging.ToString());
                stProject.AppendLine("GO");
                stProject.AppendLine(string.Format("USE [{0}]", "StandardLoggingStructure"));
                stProject.AppendLine("GO");
                stProject.AppendLine(postscriptLogging);
                stProject.AppendLine("GO");
                stProject = stProject.Replace("StandardLoggingStructure", project + "_Dev_Logging");

                stProject.Append(sbTransaction.ToString());
                stProject.AppendLine("GO");
                stProject.AppendLine(string.Format("USE [{0}]", "StandardTransactionStructure"));
                stProject.AppendLine("GO");
                stProject.AppendLine(postscriptTransaction);
                stProject.AppendLine("GO");
                stProject = stProject.Replace("StandardTransactionStructure", project + "_Dev_Transaction");

                if (lstQueueing.Contains(project))
                {
                    stProject.Append(sbQueueing.ToString());
                    stProject.AppendLine("GO");
                    stProject.AppendLine(string.Format("USE [{0}]", "StandardQueueingStructure"));
                    stProject.AppendLine("GO");
                    stProject.AppendLine(postscriptQueueing);
                    stProject.AppendLine("GO");
                    stProject = stProject.Replace("StandardQueueingStructure", project + "_Dev_Queueing");
                }

                File.WriteAllText(string.Format("{0}\\{1}.sql", sqlFilesOutputPath, project), stProject.ToString());
            }
        }

        #endregion
    }
}
