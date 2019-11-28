using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;
using Vanrise.Caching;
using Vanrise.Security.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using System.Configuration;

namespace Vanrise.Common.Business
{
    public class VRDevProjectManager : IVRDevProjectManager
    {
        //static Guid businessEntityDefinitionId = new Guid("6954527C-6DE0-411C-859D-E044165D58AF");
        static VRCompiledAssemblyManager s_compiledAssemblyManager = new VRCompiledAssemblyManager();
        static IVRDevProjectDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<IVRDevProjectDataManager>();
        static bool shouldConcatenateTitleWithDevProjectName = false;
        static VRDevProjectManager()
        {
            string shouldConcatente = ConfigurationManager.AppSettings["VRCommon_ConcatenateTitlesWithDevProjectName"];
            if (!string.IsNullOrEmpty(shouldConcatente) && bool.Parse(shouldConcatente))
                shouldConcatenateTitleWithDevProjectName = true;
        }

        #region Public Methods
        public IEnumerable<VRDevProjectInfo> GetVRDevProjectsInfo(VRDevProjectInfoFilter filter)
        {
            var vrDevProjects = GetCachedVRDevProjects();

            if (vrDevProjects == null || vrDevProjects.Count == 0)
                return null;

            Func<VRDevProject, bool> filterExpression = filterExpression = (vrDevProject) =>
            {
                if (filter == null)
                    return true;
                return true;
            };
            return vrDevProjects.Values.MapRecords(VRDevProjectInfoMapper, filterExpression);

        }
        public string GetVRDevProjectName(Guid vrDevProjectId)
        {
            var record = GetCachedVRDevProjects().GetRecord(vrDevProjectId);
            return record != null ? record.Name : null;
        }

        public VRDevProject GetVRDevProject(Guid devProjectId)
        {
            return GetCachedVRDevProjects().GetRecord(devProjectId);
        }

        public string ConcatenateTitleAndDevProjectName(Guid? devProjectId, string title)
        {
            if (!devProjectId.HasValue || !shouldConcatenateTitleWithDevProjectName)
                return title;

            var devProjectName = GetVRDevProjectName(devProjectId.Value);

            return $"{title} ({devProjectName})";
        }

        public bool TryGetDevProjectAssembly(Guid devProjectId, DateTime compiledAfter, out System.Reflection.Assembly assembly)
        {
            var devProject = GetVRDevProject(devProjectId);
            devProject.ThrowIfNull("devProject", devProjectId);

            if (devProject.AssemblyId.HasValue && devProject.AssemblyCompiledTime.HasValue && devProject.AssemblyCompiledTime.Value >= compiledAfter)
            {
                assembly = System.Reflection.Assembly.Load(devProject.AssemblyName);
                if (assembly != null)
                    return true;
            }

            CSharpCompilationOutput compilationOutput;
            if (TryCompileDevProject(devProjectId, true, devProject.AssemblyId, out compilationOutput))
            {
                devProject = GetVRDevProject(devProjectId);
                devProject.ThrowIfNull("devProject", devProjectId);
                assembly = System.Reflection.Assembly.Load(devProject.AssemblyName);
                if (assembly != null)
                    return true;
            }

            assembly = null;
            return false;
        }

        public VRDevProjectCompilationOutput TryCompileDevProject(Guid devProjectId, VRDevProjectCompilationBuildOption buildOption)
        {
            VRDevProjectCompilationOutput compilationOutput = new VRDevProjectCompilationOutput();
            CSharpCompilationOutput cSharpCompilationOutput;

            switch (buildOption)
            {
                case VRDevProjectCompilationBuildOption.Build:
                    compilationOutput.Result = TryCompileDevProject(devProjectId, out cSharpCompilationOutput);
                    if (cSharpCompilationOutput.ErrorMessages != null)
                        compilationOutput.ErrorMessages = cSharpCompilationOutput.ErrorMessages;
                    return compilationOutput;

                case VRDevProjectCompilationBuildOption.BuildWithDependencies:
                    return TryCompileDevProjetWithDependencies(devProjectId, compilationOutput, new HashSet<Guid>());

                default: return compilationOutput;
            }
        }

        public VRDevProjectCompilationOutput TryCompileDevProjetWithDependencies(Guid devProjectId, VRDevProjectCompilationOutput compilationOutput, HashSet<Guid> devProjectIds)
        {
            if (devProjectIds == null)
                devProjectIds = new HashSet<Guid>();

            devProjectIds.Add(devProjectId);

            var devProject = GetVRDevProject(devProjectId);
            if (devProject.ProjectDependencies != null && devProject.ProjectDependencies.Count > 0)
            {
                foreach (var dependency in devProject.ProjectDependencies)
                {
                    if (dependency.DependentProjectId != null && !devProjectIds.Contains(dependency.DependentProjectId))
                    {
                        if (!TryCompileDevProjetWithDependencies(dependency.DependentProjectId, compilationOutput, devProjectIds).Result)
                            return compilationOutput;
                    }
                }
            }

            CSharpCompilationOutput cSharpCompilationOutput;
            compilationOutput.Result = TryCompileDevProject(devProjectId, out cSharpCompilationOutput);

            if (!compilationOutput.Result)
            {
                compilationOutput.ModuleName = devProject.Name;

                if (cSharpCompilationOutput.ErrorMessages != null && cSharpCompilationOutput.ErrorMessages.Count > 0)
                    compilationOutput.ErrorMessages = cSharpCompilationOutput.ErrorMessages;
            }

            return compilationOutput;
        }
        public bool TryCompileDevProject(Guid devProjectId, out CSharpCompilationOutput output)
        {
            return TryCompileDevProject(devProjectId, false, null, out output);
        }

        private bool TryCompileDevProject(Guid devProjectId, bool updateProjectOnlyIfExistingAssemblyNotChanged, Guid? existingAssemblyId, out CSharpCompilationOutput output)
        {
            var generateCodeContext = new VRDevProjectCodeGeneratorGenerateCodeContext(devProjectId);
            foreach (var codeGeneratorType in Utilities.GetAllImplementations<VRDevProjectCodeGenerator>())
            {
                var codeGenerator = Activator.CreateInstance(codeGeneratorType).CastWithValidate<VRDevProjectCodeGenerator>("codeGenerator");
                codeGenerator.GenerateCode(generateCodeContext);
            }

            IUtilityDataManager utilityDataManager = CommonDataManagerFactory.GetDataManager<IUtilityDataManager>();
            DateTime compiledTime = utilityDataManager.GetNowDBTime();

            var codeFiles = new CSharpCodeFileToCompileCollection();
            foreach (var codeFile in generateCodeContext.CodeFiles)
            {
                codeFiles.Add(codeFile.Name, codeFile);
            }

            if (CSharpCompiler.TryCompileClass(
                $"{CSharpCompiler.DEVPROJECT_ASSEMBLY_PREFIX}_{devProjectId.ToString("N")}",
                CSharpCompiler.DEVPROJECT_ASSEMBLY_SUFFIX,
                codeFiles,
                new List<Guid> { devProjectId },
                out output))
            {
                Guid assemblyId = Guid.NewGuid();
                s_compiledAssemblyManager.AddAssembly(assemblyId, output.OutputAssembly.FullName, devProjectId, output.AssemblyFile, compiledTime);

                s_dataManager.UpdateProjectAssemblyId(devProjectId, assemblyId, updateProjectOnlyIfExistingAssemblyNotChanged, existingAssemblyId);
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Private Classes

        private class VRDevProjectCodeGeneratorGenerateCodeContext : IVRDevProjectCodeGeneratorGenerateCodeContext
        {
            List<CSharpCodeFileToCompile> _codeFiles = new List<CSharpCodeFileToCompile>();
            Guid _devProjectId;

            HashSet<string> _usingStatements = new HashSet<string>();
            List<string> _codes = new List<string>();

            public VRDevProjectCodeGeneratorGenerateCodeContext(Guid devProjectId)
            {
                this._devProjectId = devProjectId;
            }

            public Guid DevProjectId => _devProjectId;

            internal List<CSharpCodeFileToCompile> CodeFiles
            {
                get
                {
                    var codeFiles = new List<CSharpCodeFileToCompile>(_codeFiles);
                    if (this._codes.Count > 0)
                        codeFiles.Add(new CSharpCodeFileToCompile
                        {
                            Name = "MainCode",
                            Code = string.Concat(string.Join("\n", _usingStatements), "\n", "\n", string.Join("\n", this._codes))
                        });

                    return _codeFiles;
                    //       return _codeFiles;
                }
            }

            public void AddCode(string code)
            {
                _codes.Add(code);
            }

            public void AddCodeFile(string fileName, string code)
            {
                this._codeFiles.Add(new CSharpCodeFileToCompile
                {
                    Name = fileName,
                    Code = code
                });
            }

            public void AddUsing(string usingStatement)
            {
                _usingStatements.Add(usingStatement);
            }
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRDevProjectDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRDevProjectDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreVRDevProjectUpdated(ref _updateHandle);
            }
        }


        #endregion

        #region Private Methods

        public Dictionary<Guid, VRDevProject> GetCachedVRDevProjects()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRDevProjects", () =>
            {
                return s_dataManager.GetDevProjects().ToDictionary(itm => itm.VRDevProjectID, itm => itm);
            });
        }

        #endregion

        #region Mappers
        public VRDevProjectInfo VRDevProjectInfoMapper(VRDevProject vrDevProject)
        {
            return new VRDevProjectInfo()
            {
                VRDevProjectID = vrDevProject.VRDevProjectID,
                Name = vrDevProject.Name
            };
        }

        #endregion
    }

    public abstract class VRDevProjectCodeGenerator
    {
        public abstract void GenerateCode(IVRDevProjectCodeGeneratorGenerateCodeContext context);
    }

    public interface IVRDevProjectCodeGeneratorGenerateCodeContext
    {
        Guid DevProjectId { get; }

        void AddUsing(string usingStatement);

        void AddCode(string code);

        void AddCodeFile(string fileName, string code);
    }
    public class VRDevProjectCompilationOutput
    {
        public string ModuleName { get; set; }

        public List<string> ErrorMessages { get; set; }

        public bool Result { get; set; }
    }
}
