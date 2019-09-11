using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestRuntime;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Integration.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.Runtime.Tasks
{
    public class RodiTask : ITask
    {
        public void Execute()
        {
            #region Runtime
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationRuntimeService);

            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            DataSourceRuntimeService dsRuntimeService = new DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BigDataRuntimeService bigDataService = new BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingRuntimeService);

            CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingDistributorRuntimeService);

            DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingExecutorRuntimeService);

            DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingDistributorRuntimeService);
            #endregion
            #region comments
            /*var assemblies = new List<Assembly>();

            var tOneFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "TOne*Entities.dll");
            var vanriseFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Vanrise*Entities.dll");

            foreach (var file in tOneFiles)
                assemblies.Add(Assembly.LoadFile(file));
            foreach (var file in vanriseFiles)
                assemblies.Add(Assembly.LoadFile(file));

            Vanrise.Common.Business.UtilityManager.GenerateDocumentationForEnums(assemblies);*/


            //VRFileManager fileManager = new VRFileManager();
            //List<VRFile> files = new List<VRFile>();
            //VRFile z = new VRFile();
            //Stream stream = new MemoryStream(z.Content);
            //using (var fs = new FileStream(@"D:\rodi.zip", FileMode.Open, FileAccess.Read))
            //{
            //	using (var zf = new ZipFile(stream))
            //	{
            //		foreach (ZipEntry ze in zf)
            //		{
            //			if (ze.IsDirectory)
            //				continue;
            //			Console.Out.WriteLine(ze.Name);
            //			using (Stream s = zf.GetInputStream(ze))
            //			{
            //				byte[] buf = new byte[4096];
            //				// Analyze file in memory using MemoryStream.
            //				using (MemoryStream ms = new MemoryStream())
            //				{
            //					StreamUtils.Copy(s, ms, buf);
            //				}
            //				string[] nameastab = ze.Name.Split('.');
            //				VRFile file = new VRFile()
            //				{
            //					Content = buf,
            //					Name = ze.Name,
            //					Extension = nameastab[nameastab.Length - 1],
            //					IsTemp = true,
            //				};
            //				long id = fileManager.AddFile(file);
            //				file.FileId = id;
            //				files.Add(file);
            //				// Uncomment the following lines to store the file on disk.
            //				/*using (FileStream fs = File.Create(@"c:\temp\uncompress_" + ze.Name))
            //                         {
            //                           StreamUtils.Copy(s, fs, buf);
            //                         }*/
            //			}
            //		}
            //	}
            //}

            //VRHttpConnection x = new VRHttpConnection();
            //x.BaseURL = "Test";
            //x.Headers = new List<VRHttpHeader>();
            //string ser = Vanrise.Common.Serializer.Serialize(x);
            //string path = string.Format(@"D:\Mediation\{0}.txt", "rodi");
            //using (var fileStream = new FileStream(path, FileMode.Create))
            //{
            //	using (var tw = new StreamWriter(fileStream))
            //	{
            //		tw.Write(ser);
            //		tw.Close();
            //	}
            //}

            #endregion

            #region rodi
            //var assembliesR = new List<Assembly>();

            //var tOneFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            //foreach (var file in tOneFiles)
            //    assembliesR.Add(Assembly.LoadFile(file));

            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //var baseTypes = new List<Type>();
            //foreach (var assembly in assemblies)
            //{
            //    if (BaseAssemblies.Contains(assembly.GetName().Name))
            //        baseTypes.AddRange(assembly.DefinedTypes);
            //}

            //VRWorkflowVariableCollection vRWorkflowVariableCollection = new VRWorkflowVariableCollection();
            //VRWorkflowArgumentCollection vRWorkflowArgumentCollection = new VRWorkflowArgumentCollection();
            //TextVariablesCollection textVariablesCollection = new TextVariablesCollection();

            //vRWorkflowVariableCollection.Add(new VRWorkflowVariable { VRWorkflowVariableId = Guid.NewGuid(), Name = "var1", Type = new VRWorkflowCustomClassType() { FieldType = new VRCustomClassType() { Namespace = "System", ClassName = "Int32" } } });

            //vRWorkflowArgumentCollection.Add(new VRWorkflowArgument { VRWorkflowArgumentId = Guid.NewGuid(), Name = "Arg1", Type = new VRWorkflowCustomClassType() { FieldType = new VRCustomClassType() { Namespace = "System.Collections.Generic", ClassName = "List`1[System.Int32]" } } });


            //int position = 51;
            //string userCode = "var1. Vanrise.Common.Business.ConfigManager. String. ";

            //var res = GetSuggestions(userCode, position, vRWorkflowVariableCollection, vRWorkflowArgumentCollection, textVariablesCollection, assemblies.ToList(), baseTypes);


            //Console.WriteLine("res");
            //Console.WriteLine(res.Count);
            //foreach (var i in res)
            //    Console.WriteLine(i);

            #endregion
            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
        }

        //private SortedSet<string> GetSuggestions(string userCode, int position, VRWorkflowVariableCollection vRWorkflowVariableCollection, VRWorkflowArgumentCollection vRWorkflowArgumentCollection, TextVariablesCollection textVariablesCollection, List<Assembly> assemblies, List<Type> baseTypes)
        //{
        //    List<TextVariable> allVariables = new List<TextVariable>();
        //    foreach (var textVariable in vRWorkflowVariableCollection)
        //    {
        //        allVariables.Add(new TextVariable() { Name = textVariable.Name, VarType = textVariable.Type.GetRuntimeType(null) });
        //    }

        //    foreach (var textVariable in vRWorkflowArgumentCollection)
        //    {
        //        allVariables.Add(new TextVariable() { Name = textVariable.Name, VarType = textVariable.Type.GetRuntimeType(null) });
        //    }

        //    foreach (var textVariable in textVariablesCollection)
        //    {
        //        allVariables.Add(new TextVariable() { Name = textVariable.Name, VarType = textVariable.VarType });
        //    }


        //    var suggestions = new SortedSet<string>();
        //    var lastCharachterIndex = GetIndex(userCode, position, true);
        //    var lastCharachter = userCode[lastCharachterIndex];
        //    string lastWord = null;

        //    if (lastCharachter == '.')
        //    {
        //        var lastWordStartIndex = GetIndex(userCode, position - 1, false);
        //        if (lastWordStartIndex < lastCharachterIndex)
        //            lastWord = userCode.Substring(lastWordStartIndex + 1, lastCharachterIndex - lastWordStartIndex - 1);
        //        if (!string.IsNullOrEmpty(lastWord))
        //        {
        //            var variable = allVariables.Find(item => item.Name == lastWord);
        //            if (variable != null)
        //            {
        //                var members = variable.VarType.GetMembers();
        //                foreach (var member in members)
        //                    suggestions.Add(member.Name);
        //                return suggestions;
        //            }

        //            var baseType = baseTypes.Find(item => item.Name == lastWord);
        //            if (baseType != null)
        //            {
        //                var members = baseType.GetMembers();
        //                foreach (var member in members)
        //                    suggestions.Add(member.Name);
        //                return suggestions;
        //            }

        //            var assembly = assemblies.FirstOrDefault(item => item.GetName().Name == lastWord);
        //            if (assembly != null)
        //            {
        //                var assemblyMember = assembly.DefinedTypes;
        //                var assemblyMember2 = assembly.GetTypes();
        //                foreach (var member in assemblyMember2)
        //                    suggestions.Add(member.Name);
        //                return suggestions;
        //            }

        //            var lastWordType = Type.GetType(lastWord);

        //            if (lastWordType != null)
        //            {
        //                var members = lastWordType.GetMembers();
        //                foreach (var member in members)
        //                    suggestions.Add(member.Name);
        //                return suggestions;
        //            }
        //            else
        //            {
        //                Assembly currentAssembly = null;
        //                foreach (var assemblyA in assemblies)
        //                {
        //                    if (lastWord.StartsWith(assemblyA.GetName().Name) && (currentAssembly == null || assemblyA.GetName().Name.Length > currentAssembly.GetName().Name.Length))
        //                        currentAssembly = assemblyA;
        //                }
        //                if (currentAssembly != null)
        //                {
        //                    lastWord = String.Concat(lastWord, ", ", currentAssembly.GetName().Name);
        //                    lastWordType = Type.GetType(lastWord);
        //                    if (lastWordType != null)
        //                    {
        //                        var members = lastWordType.GetMembers();
        //                        foreach (var member in members)
        //                            suggestions.Add(member.Name);
        //                        return suggestions;
        //                    }
        //                }
        //            }

        //        }
        //    }

        //    if (!CloseSpeciaCharacterList.Contains(lastCharachter))
        //    {
        //        foreach (var variable in allVariables)
        //            suggestions.Add(variable.Name);

        //        foreach (var assembly in assemblies)
        //            suggestions.Add(assembly.GetName().Name);

        //        foreach (var baseType in baseTypes)
        //            suggestions.Add(baseType.Name);
        //    }

        //    return suggestions;
        //}

        private int GetIndex(string text, int position, bool stopAtDot)
        {
            if (position < text.Length && !String.IsNullOrEmpty(text))
            {
                while (position >= 0)
                {
                    if (text[position] == '.' && stopAtDot)
                        return position;

                    if (SpeciaCharacterList.Contains(text[position]))
                    {
                        return position;
                    }
                    position--;
                }
            }
            return 0;
        }

        private static List<Char> SpeciaCharacterList = new List<char> { '(', ')', '{', '}', '[', ']', '<', '>', '\'', '"', '\\', '/', '?', '!', '@', '#', '$', '%', '^', '&', '|', ' ', ',', ':', ';', '+', '=', '*' };
        private static List<Char> CloseSpeciaCharacterList = new List<char> { ')', '}', ']', '>', '\'', '"', '\\', '/', '@', '#', '$', '%' };
        private static List<string> BaseAssemblies = new List<string> { "System", "System.Collections.Generic", "System.Linq", "Vanrise.Common", "Vanrise.BusinessProcess", "System.Activities", "System.Activities.Statements", "Microsoft.CSharp.Activities", "mscorlib" };
    }

    public class TextVariablesCollection : List<TextVariable> { }
    public class TextVariable
    {
        public string Name { get; set; }
        public Type VarType { get; set; }
    }

    public class Result
    {
        public string Name;
        public MemberTypes ResultType;
    }
}
