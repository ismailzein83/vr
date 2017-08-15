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

namespace Vanrise.HelperTools
{
    public class Common
    {
        public static string ServerIP { get { return ConfigurationManager.AppSettings["ServerIP"]; } }
        public static string ActiveDBs { get { return ConfigurationManager.AppSettings["ActiveDBs"]; } }
        public static string SqlFilesOutputPath { get { return ConfigurationManager.AppSettings["sqlFilesOutputPath"]; } }
        public static string JavascriptsOutputPath { get { return ConfigurationManager.AppSettings["javascriptsOutputPath"]; } }
        public static string VersionDateFormat { get { return ConfigurationManager.AppSettings["VersionDateFormat"]; } }
        public static string VersionNumber { get { return ConfigurationManager.AppSettings["VersionNumber"]; } }
        public static string SQLUsername { get { return ConfigurationManager.AppSettings["SQLUsername"]; } }
        public static string SQLPassword { get { return ConfigurationManager.AppSettings["SQLPassword"]; } }

        #region CompressJS, GRPJS and GRPJSOverridden
        public static void CompressJSFiles(string currentDateShort, string folder, string javascriptsOutputPath)
        {
            ECMAScriptPacker p = new ECMAScriptPacker((ECMAScriptPacker.PackerEncoding)ECMAScriptPacker.PackerEncoding.None, false, false);

            if (string.IsNullOrEmpty(javascriptsOutputPath))
            {
                javascriptsOutputPath = Common.JavascriptsOutputPath;
            }

            var allFiles = Directory.GetFiles(string.Format(javascriptsOutputPath, currentDateShort, folder), "*.js", SearchOption.AllDirectories);

            string ExtraFilename = string.Empty;

            Parallel.ForEach(allFiles, file =>
            {
                CompressFile(file, ExtraFilename, p);
            });
        }

        public static void CompressFile(string file, string ExtraFilename, ECMAScriptPacker p)
        {
            File.WriteAllText(string.Format("{0}\\{1}{2}{3}", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file), ExtraFilename, Path.GetExtension(file)), p.Pack(File.ReadAllText(Path.GetFullPath(file))));
        }

        public static void GroupJSFiles(string currentDateShort, string folder, bool overridden, string javascriptsOutputPath)
        {
            if (string.IsNullOrEmpty(javascriptsOutputPath))
            {
                javascriptsOutputPath = Common.JavascriptsOutputPath;
            }
            var allDirectories = Directory.GetDirectories(string.Format(javascriptsOutputPath, currentDateShort, folder), "*", SearchOption.TopDirectoryOnly);

            Parallel.ForEach(allDirectories, directory =>
            {
                var allFiles = Directory.GetFiles(directory, "*.js", SearchOption.AllDirectories);

                var orgDirectoryName = Path.GetFileName(directory);
                var directoryName = overridden ? string.Format("{0}{1}", orgDirectoryName, "_Overridden") : orgDirectoryName;

                //create file if not exisit
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



        #endregion

        #region GRPSQL and GRPSQLOverridden
        public static void GroupSQLPostScriptFiles(string currentDateShort, bool overridden, string sqlFilesOutputPath)
        {
            if (string.IsNullOrEmpty(sqlFilesOutputPath))
            {
                sqlFilesOutputPath = Common.SqlFilesOutputPath;
            }

            var allDirectories = Directory.GetDirectories(string.Format(sqlFilesOutputPath, currentDateShort, ""), "*", SearchOption.TopDirectoryOnly);

            foreach (var directory in allDirectories)
            {
                var orgDirectoryName = Path.GetFileName(directory);
                var directoryName = overridden ? string.Format("{0}{1}", orgDirectoryName, "_Overridden") : orgDirectoryName;

                //create file if not exisit
                if (!File.Exists(string.Format("{0}\\{1}{2}", directory, directoryName, ".sql")))
                {
                    StringBuilder fileContent = new StringBuilder();

                    string[] allFiles = Directory.GetFiles(directory, "*.sql", SearchOption.AllDirectories); ;
                    Array.Sort(allFiles);
                    if (File.Exists(string.Format("{0}\\{1}{2}", directory, orgDirectoryName, ".txt")))
                    {
                        allFiles = File.ReadAllLines(string.Format("{0}\\{1}{2}", directory, orgDirectoryName, ".txt"));
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

        public static void GenerateDBStructure(string currentDate, string currentDateShort, List<string> lstDBs, string sqlFilesOutputPath)
        {
            if (lstDBs.Count < 1)
            {
                lstDBs.AddRange(Common.ActiveDBs.Split('#'));
            }
            if (string.IsNullOrEmpty(sqlFilesOutputPath))
            {
                sqlFilesOutputPath = Common.SqlFilesOutputPath;
            }

            Parallel.ForEach(lstDBs, item =>
            {
                Console.WriteLine(string.Format("Creating database structure for: {0}", item));
                GenerateSQLDBScript(item, string.Format(sqlFilesOutputPath, currentDateShort, "DBsStructure"), currentDate, currentDateShort);
                Console.WriteLine(string.Format("Finish creating database structure for: {0}", item));
            });
        }

        public static void GenerateSQLDBScript(string item, string sqlFilesOutputPath, string currentDate, string currentDateShort)
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

            sb = sb.Replace(item, string.Format("{0}_{1}", item, currentDateShort));
            if (!Directory.Exists(sqlFilesOutputPath))
            {
                Directory.CreateDirectory(sqlFilesOutputPath);
            }
            File.WriteAllText(string.Format("{0}\\{1}_{2}.sql", sqlFilesOutputPath, item, currentDate), sb.ToString());
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
