﻿using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchReleaseCauseManager
    {
     
        SwitchManager switchManager = new SwitchManager();
      
        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SwitchReleaseCauseDetail> GetFilteredSwitchReleaseCauses(Vanrise.Entities.DataRetrievalInput<SwitchReleaseCauseQuery> input)
        {
            var allSwitchReleaseCauses = this.GetCachedSwitchReleaseCauses();

            Func<SwitchReleaseCause, bool> filterExpression = (prod) =>
             (input.Query.ReleaseCode == null || prod.ReleaseCode.ToLower().Contains(input.Query.ReleaseCode.ToLower())) &&

                (input.Query.SwitchIds == null || input.Query.SwitchIds.Contains(prod.SwitchId)) &&
                (!input.Query.IsDelivered.HasValue || input.Query.IsDelivered.Value == (prod.Settings.IsDelivered)) &&
                (input.Query.Description == null || prod.Settings.Description.ToLower().Contains(input.Query.Description.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSwitchReleaseCauses.ToBigResult(input, filterExpression, SwitchReleaseCauseDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<SwitchReleaseCauseDetail> AddSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            int switchReleaseCauseId = -1;
            InsertOperationOutput<SwitchReleaseCauseDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchReleaseCauseDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
            bool insertActionSucc = dataManager.AddSwitchReleaseCause(switchReleaseCause, out switchReleaseCauseId);
            if (insertActionSucc)
            {
                switchReleaseCause.SwitchReleaseCauseId = switchReleaseCauseId;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SwitchReleaseCauseDetailMapper(switchReleaseCause);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            return insertOperationOutput;
        }
        public SwitchReleaseCause GetSwitchReleaseCause(int switchReleaseCauseId)
        {
            var allSwitchReleaseCauses = this.GetCachedSwitchReleaseCauses();
            return allSwitchReleaseCauses.GetRecord(switchReleaseCauseId);
        }
        public Vanrise.Entities.UpdateOperationOutput<SwitchReleaseCauseDetail> UpdateSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            UpdateOperationOutput<SwitchReleaseCauseDetail> updateOperationOutput = new UpdateOperationOutput<SwitchReleaseCauseDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
            bool updateActionSucc = dataManager.UpdateSwitchReleaseCause(switchReleaseCause);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchReleaseCauseDetailMapper(switchReleaseCause);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public UploadSwitchReleaseCauseLog AddSwitchReleaseCauses(int fileId, int switchId)
        {
            UploadSwitchReleaseCauseLog uploadSwitchReleaseCauseLog = new UploadSwitchReleaseCauseLog();
            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFile(fileId).Content;
            var fileStream = new System.IO.MemoryStream(bytes);
            //ExportTableOptions options = new ExportTableOptions();
            //options.CheckMixedValueType = true;
            Workbook wbk = new Workbook(fileStream);
            Worksheet worksheet = wbk.Worksheets[0];
            List<String> headers = new List<string>();
            headers.Add(worksheet.Cells[0, 0].StringValue);
            headers.Add("Result");
            headers.Add("Error Message");
            //wbk.CalculateFormula();
            List<SwitchReleaseCause> addedSwitchReleaseCauses = new List<SwitchReleaseCause>();
            int count = 1;
            while (count < worksheet.Cells.Rows.Count)
            {
                var switchReleaseCause = new SwitchReleaseCause()
                {
                    ReleaseCode =  worksheet.Cells[count, 0].StringValue.Trim(),
                    SwitchId = switchId
                };
                switchReleaseCause.Settings = new SwitchReleaseCauseSetting();
                if (worksheet.Cells[count, 2].StringValue.Trim() == "Y")
                    switchReleaseCause.Settings.IsDelivered = true;
                else if (worksheet.Cells[count, 2].StringValue.Trim() == "N")
                    switchReleaseCause.Settings.IsDelivered = false;
                switchReleaseCause.Settings.Description = worksheet.Cells[count, 1].StringValue.Trim();
                addedSwitchReleaseCauses.Add(switchReleaseCause);
                count++;
            }
            Workbook returnedExcel = new Workbook();
            Vanrise.Common.Utilities.ActivateAspose();
            returnedExcel.Worksheets.Clear();
            Worksheet SwitchReleaseCauseWorkSheet = returnedExcel.Worksheets.Add("Result");
            int rowIndex = 0;
            int colIndex = 0;
            foreach (var header in headers)
            {
                SwitchReleaseCauseWorkSheet.Cells.SetColumnWidth(colIndex, 20);
                SwitchReleaseCauseWorkSheet.Cells[rowIndex, colIndex].PutValue(header);
                Cell cell = SwitchReleaseCauseWorkSheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0);
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }
            rowIndex++;
            colIndex = 0;
            foreach (var addedSwitchReleaseCause in addedSwitchReleaseCauses)
            {
                SwitchReleaseCauseWorkSheet.Cells[rowIndex, colIndex].PutValue(addedSwitchReleaseCause.ReleaseCode);
                colIndex++;

                SwitchReleaseCause switchReleaseCause = GetCachedSwitchReleaseCauses().FindRecord(it => it.ReleaseCode.Equals(addedSwitchReleaseCause.ReleaseCode, StringComparison.InvariantCultureIgnoreCase) && it.SwitchId.Equals(addedSwitchReleaseCause.SwitchId));
                if (!String.IsNullOrEmpty(addedSwitchReleaseCause.ReleaseCode))
                {
                    if (switchReleaseCause == null)
                    {
                        int switchReleaseCauseId = -1;
                        ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
                        bool insertActionSucc = dataManager.AddSwitchReleaseCause(addedSwitchReleaseCause, out switchReleaseCauseId);
                        if (insertActionSucc)
                        {
                            SwitchReleaseCauseWorkSheet.Cells[rowIndex, colIndex].PutValue("Succeed");
                            uploadSwitchReleaseCauseLog.CountOfSwitchReleaseCausesAdded++;
                            colIndex = 0;
                            rowIndex++;
                        }
                        else
                        {
                            SwitchReleaseCauseWorkSheet.Cells[rowIndex, colIndex].PutValue("Failed");
                            colIndex++;
                            SwitchReleaseCauseWorkSheet.Cells[rowIndex, colIndex].PutValue("SwitchReleaseCause already exists");
                            uploadSwitchReleaseCauseLog.CountOfSwitchReleaseCausesExist++;
                            colIndex = 0;
                            rowIndex++;
                        }
                    }
                    else
                    {
                        SwitchReleaseCauseWorkSheet.Cells[rowIndex, colIndex].PutValue("Failed");
                        colIndex++;
                        SwitchReleaseCauseWorkSheet.Cells[rowIndex, colIndex].PutValue("Country already exists");
                        uploadSwitchReleaseCauseLog.CountOfSwitchReleaseCausesExist++;
                        colIndex = 0;
                        rowIndex++;
                    }
                }
                else
                    colIndex = 0;
            }

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = returnedExcel.SaveToStream();

            VRFile saveFile = new VRFile()
            {
                Content = memoryStream.ToArray(),
                Name = "SwitchReleaseCauseLog",
                CreatedTime = DateTime.Now,
                Extension = ".xlsx"
            };
            VRFileManager manager = new VRFileManager();
            uploadSwitchReleaseCauseLog.fileID = manager.AddFile(saveFile);

            return uploadSwitchReleaseCauseLog;
        }
        public byte[] DownloadSwitchReleaseCauseLog(long fileID)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileID);
            return file.Content;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreSwitchReleaseCausesUpdated(ref _updateHandle);
            }
        }
        #endregion
     
        #region Mappers
        private SwitchReleaseCauseDetail SwitchReleaseCauseDetailMapper(SwitchReleaseCause switchReleaseCause)
        {
            var switchReleaseCauseDetail = new SwitchReleaseCauseDetail()
            {
                SwitchReleaseCauseId = switchReleaseCause.SwitchReleaseCauseId,
                ReleaseCode = switchReleaseCause.ReleaseCode,
                SwitchId = switchReleaseCause.SwitchId,
                SwitchName = switchManager.GetSwitchName(switchReleaseCause.SwitchId),

            };

            switchReleaseCauseDetail.Description = switchReleaseCause.Settings.Description;
            switchReleaseCauseDetail.IsDelivered = switchReleaseCause.Settings.IsDelivered;



            return switchReleaseCauseDetail;
        }
        #endregion
     
        #region Private Methods
        Dictionary<int, SwitchReleaseCause> GetCachedSwitchReleaseCauses()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchReleaseCauses",
               () =>
               {
                   ISwitchReleaseCauseDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchReleaseCauseDataManager>();
                   IEnumerable<SwitchReleaseCause> switchReleaseCauses = dataManager.GetSwitchReleaseCauses();
                   return switchReleaseCauses.ToDictionary(cn => cn.SwitchReleaseCauseId, cn => cn);
               });
        }
        #endregion

 
    }
}
