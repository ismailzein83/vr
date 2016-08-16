using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodeGroupManager
    {
        #region ctor/Local Variables
        private readonly CountryManager _countryManager;
        public CodeGroupManager()
        {
            _countryManager = new CountryManager();
        }
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<CodeGroupDetail> GetFilteredCodeGroups(Vanrise.Entities.DataRetrievalInput<CodeGroupQuery> input)
        {
            var allCodeGroups = GetCachedCodeGroups();

            Func<CodeGroup, bool> filterExpression = (prod) =>
                 (input.Query.Code == null || prod.Code.ToLower().Contains(input.Query.Code.ToLower()))
                  &&
                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count() == 0 || input.Query.CountriesIds.Contains(prod.CountryId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeGroups.ToBigResult(input, filterExpression, CodeGroupDetailMapper));
        }
        public CodeGroup GetMatchCodeGroup(string code)
        {
            CodeIterator<CodeGroup> codeIterator = GetCodeIterator();
            return codeIterator.GetLongestMatch(code);
        }
        public IEnumerable<CodeGroupInfo> GetAllCodeGroups()
        {
            return GetCachedCodeGroups().MapRecords(CodeGroupInfoMapper).OrderBy(x => x.Name);
        }
        public CodeGroup GetCodeGroup(int codeGroupId)
        {
            var codeGroups = GetCachedCodeGroups();
            return codeGroups.GetRecord(codeGroupId);
        }

        public IEnumerable<CodeGroup> GetCountryCodeGroups(int countryId)
        {
             return GetCachedCodeGroupsByCountry().GetRecord(countryId);
        }

        public TOne.Entities.InsertOperationOutput<CodeGroupDetail> AddCodeGroup(CodeGroup codeGroup)
        {
            ValidateCodeGroupToAdd(codeGroup);

            TOne.Entities.InsertOperationOutput<CodeGroupDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CodeGroupDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int coudeGroupId = -1;

            ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
            bool insertActionSucc = dataManager.Insert(codeGroup, out coudeGroupId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                codeGroup.CodeGroupId = coudeGroupId;
                insertOperationOutput.InsertedObject = CodeGroupDetailMapper(codeGroup);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public TOne.Entities.UpdateOperationOutput<CodeGroupDetail> UpdateCodeGroup(CodeGroupToEdit codeGroup)
        {
            ValidateCodeGroupToEdit(codeGroup);

            ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();

            bool updateActionSucc = dataManager.Update(codeGroup);
            TOne.Entities.UpdateOperationOutput<CodeGroupDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CodeGroupDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CodeGroupDetailMapper(this.GetCodeGroup(codeGroup.CodeGroupId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        public UploadCodeGroupLog UploadCodeGroupList(int fileId)
        {
            UploadCodeGroupLog uploadCodeGroupLog = new UploadCodeGroupLog();
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileId);
            byte[] bytes = file.Content;
            MemoryStream memStreamRate = new MemoryStream(bytes);
            Workbook objExcel = new Workbook(memStreamRate);
            Worksheet worksheet = objExcel.Worksheets[0];
            int count = 1;
            List<String> headers = new List<string>();
            headers.Add(worksheet.Cells[0, 0].StringValue);
            headers.Add(worksheet.Cells[0, 1].StringValue);
            headers.Add("Result");
            headers.Add("Error Message");
            Dictionary<string, string> addedCountriesByCodeGroup = new Dictionary<string, string>();
            while (count < worksheet.Cells.Rows.Count)
            {
                string codeGroup = worksheet.Cells[count, 0].StringValue.Trim();
                string country = worksheet.Cells[count, 1].StringValue.Trim();
                if (!addedCountriesByCodeGroup.ContainsKey(codeGroup))
                    addedCountriesByCodeGroup.Add(codeGroup, country);
                count++;
            }



            //Return Excel Result
            Workbook returnedExcel = new Workbook();
            Vanrise.Common.Utilities.ActivateAspose();
            returnedExcel.Worksheets.Clear();
            Worksheet RateWorkSheet = returnedExcel.Worksheets.Add("Result");
            int rowIndex = 0;
            int colIndex = 0;

            foreach (var header in headers)
            {

                RateWorkSheet.Cells.SetColumnWidth(colIndex, 20);
                RateWorkSheet.Cells[rowIndex, colIndex].PutValue(header);
                Cell cell = RateWorkSheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }
            rowIndex++;
            colIndex = 0;


            CountryManager countryManager = new CountryManager();
            Dictionary<string, CodeGroup> cachedCodeGroups = GetCachedCodeGroupsByCode();

            List<CodeGroup> importedCodeGroup = new List<CodeGroup>();

            foreach (var code in addedCountriesByCodeGroup)
            {
                RateWorkSheet.Cells[rowIndex, colIndex].PutValue(code.Key);
                colIndex++;
                RateWorkSheet.Cells[rowIndex, colIndex].PutValue(code.Value);
                colIndex++;

                Country country = countryManager.GetCountry(code.Value.ToLower());
                CodeGroup codeGroup = null;
               
               if (country == null || String.IsNullOrEmpty(code.Key))
                {
                    //TODO: Code Group Is Empty validation mist preceed the validation on numberic value to be more precise
                    RateWorkSheet.Cells[rowIndex, colIndex].PutValue("Failed");

                    colIndex++;
                    if (country == null && codeGroup != null)
                        RateWorkSheet.Cells[rowIndex, colIndex].PutValue("Country Not Exists and CodeGroup Exists");
                    else if (country == null && codeGroup == null)
                        RateWorkSheet.Cells[rowIndex, colIndex].PutValue("Country Not Exists");
                    else if (country != null && codeGroup != null)
                        RateWorkSheet.Cells[rowIndex, colIndex].PutValue("CodeGroup Exists");
                    else if (country != null && String.IsNullOrEmpty(code.Key))
                        RateWorkSheet.Cells[rowIndex, colIndex].PutValue("CodeGroup Is Empty");
                    uploadCodeGroupLog.CountOfCodeGroupsFailed++;
                    colIndex = 0;
                    rowIndex++;
                }
                else if(!Vanrise.Common.Utilities.IsNumeric(code.Key, 0))
                {
                    RateWorkSheet.Cells[rowIndex, colIndex].PutValue("Failed");
                    colIndex++;
                    RateWorkSheet.Cells[rowIndex, colIndex].PutValue("CodeGroup must be a positive number");
                    uploadCodeGroupLog.CountOfCodeGroupsFailed++;
                    colIndex = 0;
                    rowIndex++;
                }

               else if (!cachedCodeGroups.TryGetValue(code.Key, out codeGroup))
               {
                   importedCodeGroup.Add(new CodeGroup
                   {
                       Code = code.Key,
                       CountryId = country.CountryId
                   });
                   uploadCodeGroupLog.CountOfCodeGroupsAdded++;
                   RateWorkSheet.Cells[rowIndex, colIndex].PutValue("Succeed");
                   colIndex = 0;
                   rowIndex++;
               }
    
            }

            ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
            dataManager.SaveCodeGroupToDB(importedCodeGroup);

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = returnedExcel.SaveToStream();

            VRFile saveFile = new VRFile()
            {
                Content = memoryStream.ToArray(),
                Name = "CodeGroupLog",
                CreatedTime = DateTime.Now,
                Extension = ".xlsx"
            };
            VRFileManager manager = new VRFileManager();
            uploadCodeGroupLog.fileID = manager.AddFile(saveFile);

            return uploadCodeGroupLog;
        }
        public byte[] DownloadCodeGroupLog(long fileID)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileID);
            return file.Content;
        }
        public byte[] DownloadCodeGroupListTemplate()
        {
            string physicalFilePath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["DownloadCodeGroupTemplatePath"]);

            byte[] bytes = File.ReadAllBytes(physicalFilePath);

            return bytes;
        }
        #endregion

        #region Validation Methods

        void ValidateCodeGroupToAdd(CodeGroup codeGroup)
        {
            ValidateCodeGroup(codeGroup.Code, codeGroup.CountryId);
        }

        void ValidateCodeGroupToEdit(CodeGroupToEdit codeGroup)
        {
            ValidateCodeGroup(codeGroup.Code, codeGroup.CountryId);
        }

        void ValidateCodeGroup(string cgCode, int cgCountryId)
        {
            if (String.IsNullOrWhiteSpace(cgCode))
                throw new MissingArgumentValidationException("CodeGroup.Code");

            var countryManager = new CountryManager();
            var country = countryManager.GetCountry(cgCountryId);
            if (country == null)
                throw new DataIntegrityValidationException(String.Format("Country '{0}' does not exist", cgCountryId));
        }

        #endregion

        #region Private Members
        private Dictionary<int, CodeGroup> GetCachedCodeGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCodeGroups",
               () =>
               {
                   ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
                   IEnumerable<CodeGroup> codegroups = dataManager.GetCodeGroups();
                   return codegroups.ToDictionary(cg => cg.CodeGroupId, cg => cg);
               });
        }
        private Dictionary<string, CodeGroup> GetCachedCodeGroupsByCode()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCodeGroupsByCode",
               () =>
               {
                   ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
                   IEnumerable<CodeGroup> codegroups = dataManager.GetCodeGroups();
                   return codegroups.ToDictionary(cg => cg.Code, cg => cg);
               });
        }

        private Dictionary<int, List<CodeGroup>> GetCachedCodeGroupsByCountry()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCodeGroupsByCountry",
               () =>
               {
                   Dictionary<int, CodeGroup> cachedCodeGroups = this.GetCachedCodeGroups();
                   if (cachedCodeGroups.Values == null)
                       return null;

                   Dictionary<int, List<CodeGroup>> codeGroupsByCountry = new Dictionary<int, List<CodeGroup>>();

                   foreach (CodeGroup cg in cachedCodeGroups.Values)
                   {
                       List<CodeGroup> codeGroups = null;
                       if (!codeGroupsByCountry.TryGetValue(cg.CountryId, out codeGroups))
                       {
                           codeGroups = new List<CodeGroup>();
                           codeGroupsByCountry.Add(cg.CountryId, codeGroups);
                       }

                       codeGroups.Add(cg);
                   }

                   return codeGroupsByCountry;                   
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICodeGroupDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCodeGroupUpdated(ref _updateHandle);
            }
        }
        private CodeIterator<CodeGroup> GetCodeIterator()
        {
            var cachedCodeGroups = GetCachedCodeGroups();
            return new CodeIterator<CodeGroup>(cachedCodeGroups.Values);
        }
        #endregion

        #region  Mappers
        private CodeGroupDetail CodeGroupDetailMapper(CodeGroup codeGroup)
        {
            CodeGroupDetail codeGroupDetail = new CodeGroupDetail();
            codeGroupDetail.Entity = codeGroup;


            codeGroupDetail.CountryName = _countryManager.GetCountryName(codeGroup.CountryId);

            return codeGroupDetail;
        }

        private CodeGroupInfo CodeGroupInfoMapper(CodeGroup codeGroup)
        {
            string countryName = _countryManager.GetCountryName(codeGroup.CountryId);
            return new CodeGroupInfo()
            {
                CodeGroupId = codeGroup.CodeGroupId,
                Name = string.Format(@"{0}{1}", countryName != null ? string.Format(@"{0} - ",countryName) : "", codeGroup.Code)
            };
        }
        #endregion
    }
}
