using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodePreparationManager
    {
        //public bool UploadSaleZonesList(int sellingNumberPlanId, int fileId,DateTime effectiveDate)
        //{

        //    VRFileManager fileManager = new VRFileManager();
        //    VRFile file = fileManager.GetFile(fileId);
        //    byte[] bytes = file.Content;
        //    MemoryStream memStreamRate = new MemoryStream(bytes);
        //    Workbook objExcel = new Workbook(memStreamRate);
        //    Worksheet worksheet = objExcel.Worksheets[0];
        //    int count=1;
        //    Dictionary<string, List<SaleCode>> newCodePreparation = new Dictionary<string, List<SaleCode>>();
        //    Dictionary<string, List<SaleCode>> deletedCodePreparation = new Dictionary<string, List<SaleCode>>();
        //    while (count < worksheet.Cells.Rows.Count)
        //    {
        //        SaleCode saleCode = new SaleCode
        //        {
        //            Code = worksheet.Cells[count, 1].StringValue,
        //            BeginEffectiveDate = effectiveDate,
        //            EndEffectiveDate = null,
        //        };
        //        string ZoneName = worksheet.Cells[count, 0].StringValue;
        //        if ((ImportType)worksheet.Cells[count, 2].IntValue == ImportType.New)
        //        {
        //            List<SaleCode> codesList = null;
        //            if (!newCodePreparation.TryGetValue(ZoneName, out codesList))
        //            {
        //                codesList = new List<SaleCode>();
        //                codesList.Add(saleCode);
        //                newCodePreparation.Add(ZoneName, codesList);
        //            }
        //            else
        //            {
        //                if (codesList == null)
        //                    codesList = new List<SaleCode>();
        //                codesList.Add(saleCode);
        //                newCodePreparation[ZoneName] = codesList;
        //            }
                 
        //        }
        //        else if ((ImportType)worksheet.Cells[count, 2].IntValue == ImportType.Delete)
        //        {
        //            List<SaleCode> codesList = null;
        //            if (!deletedCodePreparation.TryGetValue(ZoneName, out codesList))
        //            {
        //                codesList = new List<SaleCode>();
        //                codesList.Add(saleCode);
        //                deletedCodePreparation.Add(ZoneName, codesList);
        //            }
        //            else
        //            {
        //                if (codesList == null)
        //                    codesList = new List<SaleCode>();
        //                codesList.Add(saleCode);
        //                deletedCodePreparation[ZoneName] = codesList;
        //            }
        //        }
        //        count++; 
        //    }
        //    SaleZoneManager saleZoneManager = new SaleZoneManager();
        //    Dictionary<string, List<SaleCode>> saleZonesWithCodes = saleZoneManager.GetSaleZonesWithCodes(sellingNumberPlanId,effectiveDate);
        //    List<SaleZone> saleZones = new List<SaleZone>();
        //    saleZones = saleZoneManager.GetSaleZones(sellingNumberPlanId,effectiveDate);
        //    Dictionary<string, List<SaleCode>> newImportedList = new Dictionary<string, List<SaleCode>>();
        //    List<SaleZone> newSaleZones = GetSaleZonesList(sellingNumberPlanId, effectiveDate, newCodePreparation, newImportedList, ImportType.New, saleZonesWithCodes, saleZones);
        //    Dictionary<string, List<SaleCode>> deletedList = new Dictionary<string, List<SaleCode>>();
        //    List<SaleZone> deletedSaleZones = GetSaleZonesList(sellingNumberPlanId, effectiveDate, deletedCodePreparation, deletedList, ImportType.Delete, saleZonesWithCodes, saleZones);
        //    saleZoneManager.DeleteSaleZones(deletedSaleZones);
        //    saleZoneManager.InsertSaleZones(newSaleZones);
           
            
            
        //    saleZones = saleZoneManager.GetSaleZones(sellingNumberPlanId, effectiveDate);
        //    List<SaleCode> newSaleCodes = new List<SaleCode>();
        //    List<SaleCode> deletedSaleCodes = new List<SaleCode>();
        //    SaleCodeManager saleCodeManager = new SaleCodeManager();

        //    foreach (SaleZone saleZone in saleZones){
        //        List<SaleCode> deletedSaleCodeList = null;
        //        List<SaleCode> newSaleCodeList=null;
        //        if (newImportedList.TryGetValue(saleZone.Name, out newSaleCodeList))
        //            foreach (SaleCode code in newSaleCodeList)
        //                newSaleCodes.Add(new SaleCode
        //                {
        //                    ZoneId = saleZone.SaleZoneId,
        //                    Code = code.Code,
        //                    BeginEffectiveDate = effectiveDate,
        //                    EndEffectiveDate = null
        //                });
        //        else if(deletedList.TryGetValue(saleZone.Name, out deletedSaleCodeList)){
        //           List<SaleCode> codesByZoneId= saleCodeManager.GetSaleCodesByZoneID(saleZone.SaleZoneId,effectiveDate);
        //           foreach (SaleCode code in codesByZoneId)
        //            {
        //                foreach (SaleCode codeDeleted in deletedSaleCodeList)
        //                {
        //                    if (codeDeleted.Code == code.Code)
        //                    {
        //                        deletedSaleCodes.Add(new SaleCode
        //                        {
        //                            SaleCodeId = code.SaleCodeId,
        //                            EndEffectiveDate = effectiveDate
        //                        });
        //                    }
        //                }
                       
        //            }

        //        }
                    
                       
        //    }
        //    saleCodeManager.DeleteSaleCodes(deletedSaleCodes);
        //    saleCodeManager.InsertSaleCodes(newSaleCodes);
        //    return true;
        //}
        //private List<SaleZone> GetSaleZonesList(int sellingNumberPlanId, DateTime effectiveDate, Dictionary<string, List<SaleCode>> zoneByCodesDictionary, Dictionary<string,
        //    List<SaleCode>> importedList, ImportType type, Dictionary<string, List<SaleCode>> saleZonesWithCodes, List<SaleZone> saleZones)
        //{
        //    List<SaleZone> saleZonesList = new List<SaleZone>();
        //    foreach (var obj in zoneByCodesDictionary)
        //    {
        //        switch (type)
        //        {
        //            case ImportType.New:
        //                if (!importedList.ContainsKey(obj.Key) && !saleZonesWithCodes.ContainsKey(obj.Key))
        //                {
        //                    saleZonesList.Add(new SaleZone
        //                    {
        //                        Name = obj.Key,
        //                        SellingNumberPlanId = sellingNumberPlanId,
        //                        BeginEffectiveDate = effectiveDate,
        //                        EndEffectiveDate = null
        //                    });
        //                    importedList.Add(obj.Key, obj.Value);
        //                }
        //                break;
        //            case ImportType.Delete:
        //                List<SaleCode> saleCodesList = null;
        //                if (saleZonesWithCodes.TryGetValue(obj.Key, out saleCodesList))
        //                {
        //                         SaleZone saleZone = saleZones.Find(x => x.Name == obj.Key);
        //                         saleZonesList.Add(new SaleZone
        //                        {
        //                                SaleZoneId = saleZone.SaleZoneId,
        //                                Name = obj.Key,
        //                                SellingNumberPlanId = sellingNumberPlanId,
        //                                EndEffectiveDate = effectiveDate
        //                        });
        //                         foreach (SaleCode deletedCode in saleCodesList)
        //                        {
        //                             deletedCode.EndEffectiveDate = effectiveDate;
        //                        }
        //                         importedList.Add(obj.Key, saleCodesList);
        //                 }
        //                break;
        //        }
                

        //    }
        //    return saleZonesList;
        //}

    }
}
