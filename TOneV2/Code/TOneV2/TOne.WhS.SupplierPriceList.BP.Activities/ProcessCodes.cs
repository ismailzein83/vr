using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    #region Arguments Classes
    public class ProcessCodesInput
    {
        public List<Zone> Zones { get; set; }
        public DateTime? MinimumDate { get; set; }
        public int SupplierId { get; set; }
        public List<Code> CodesToBeDeleted { get; set; }
    }

    #endregion
    public class ProcessCodes : BaseAsyncActivity<ProcessCodesInput>
    {
        public InOutArgument<List<Zone>> Zones { get; set; }
        public InArgument<DateTime?> MinimumDate { get; set; }
        public InArgument<int> SupplierId { get; set; }

        public OutArgument<List<Code>> CodesToBeDeleted { get; set; }
        protected override void DoWork(ProcessCodesInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime startPreparing = DateTime.Now;

            SupplierCodeManager manager = new SupplierCodeManager();
            List<SupplierCode> existingCodes = manager.GetSupplierCodesEffectiveAfter(inputArgument.SupplierId, (DateTime)inputArgument.MinimumDate);
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            CodesByCode codesByCode = new CodesByCode();
            foreach (SupplierCode code in existingCodes)
            {
              
               var codeGroup=  codeGroupManager.GetMatchCodeGroup(code.Code);
               if (codeGroup == null)
               {
                   handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Error, "The code:{0} doesn't belong to any codegroup", code.Code);
                   throw new WorkflowApplicationAbortedException();
               }

               List<Code> codes = null;
                if (!codesByCode.TryGetValue(code.Code, out codes))
                {
                    codes = new List<Code>();
                    codes.Add(new Code
                    {
                        BeginEffectiveDate = code.BeginEffectiveDate,
                        EndEffectiveDate = code.EndEffectiveDate,
                        CodeValue=code.Code,
                        SupplierCodeId=code.SupplierCodeId,
                        ZoneId = code.ZoneId,
                        CodeGroupId = codeGroup.CodeGroupId
                         
                    });
                    codesByCode.Add(code.Code, codes);
                }
                else
                {
                    codes.Add(new Code
                    {
                        BeginEffectiveDate = code.BeginEffectiveDate,
                        EndEffectiveDate = code.EndEffectiveDate,
                        CodeValue = code.Code,
                        ZoneId = code.ZoneId,
                        SupplierCodeId = code.SupplierCodeId,
                        CodeGroupId = codeGroup.CodeGroupId

                    });
                }
            }

            foreach (Zone zone in inputArgument.Zones)
            {
                if (zone.Codes == null || zone.Codes.Count == 0)
                {
                    zone.Codes = new List<Code>();
                }
                if (zone.Status == TOne.WhS.SupplierPriceList.Entities.Status.New)
                {
                    foreach(PriceListCodeItem code in zone.NewCodes){
                        var codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code);
                        if (codeGroup == null)
                        {
                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Error, "The code:{0} doesn't belong to any codegroup", code.Code);
                            throw new WorkflowApplicationAbortedException();
                        }
                        zone.CountryId = codeGroup.CountryId;
                        zone.Codes.Add(new Code
                    {
                        BeginEffectiveDate = code.BED,
                        EndEffectiveDate = code.EED,
                        CodeValue=code.Code,
                        ZoneId = zone.SupplierZoneId,
                        Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                        CodeGroupId = codeGroup.CodeGroupId
                    });
                    }
                    
                }
                else if (zone.Status == TOne.WhS.SupplierPriceList.Entities.Status.NotChanged)
                {
                    foreach(PriceListCodeItem code in zone.NewCodes){
                        var codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code);
                        if (codeGroup == null)
                        {
                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Error, "The code:{0} doesn't belong to any codegroup", code.Code);
                            throw new WorkflowApplicationAbortedException();
                        }
                        zone.CountryId = codeGroup.CountryId;
                          List<Code> matchedCodes = null;
                          codesByCode.TryGetValue(code.Code, out matchedCodes);

                          if (matchedCodes == null)
                          {
                              zone.Codes.Add(new Code
                              {
                                  BeginEffectiveDate = code.BED,
                                  EndEffectiveDate = code.EED,
                                  Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                                  CodeValue = code.Code,
                                  ZoneId = zone.SupplierZoneId,
                                  CodeGroupId = codeGroup.CodeGroupId
                              });
                          }
                          else
                          {
                              foreach (Code matchedCode in matchedCodes)
                              {
                                 
                                  if (matchedCode.ZoneId == zone.SupplierZoneId && matchedCode.BeginEffectiveDate == code.BED && matchedCode.EndEffectiveDate == code.EED)
                                  {
                                      zone.Codes.Add(new Code
                                      {
                                          BeginEffectiveDate = matchedCode.BeginEffectiveDate,
                                          EndEffectiveDate = matchedCode.EndEffectiveDate,
                                          Status = TOne.WhS.SupplierPriceList.Entities.Status.NotChanged,
                                          CodeValue = matchedCode.CodeValue,
                                          ZoneId = matchedCode.ZoneId,
                                          SupplierCodeId = matchedCode.SupplierCodeId,
                                          CodeGroupId = matchedCode.CodeGroupId
                                      });
                                      codesByCode.Remove(code.Code);

                                  }else if (matchedCode.ZoneId == zone.SupplierZoneId)
                                  {
                                      
                                      if (matchedCode.BeginEffectiveDate == code.BED && (matchedCode.EndEffectiveDate < code.EED || matchedCode.EndEffectiveDate > code.EED || code.EED == null))
                                      {
                                          zone.Codes.Add(new Code
                                          {
                                              BeginEffectiveDate = matchedCode.BeginEffectiveDate,
                                              EndEffectiveDate = code.EED,
                                              Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated,
                                              CodeValue = matchedCode.CodeValue,
                                              ZoneId = matchedCode.ZoneId,
                                              SupplierCodeId = matchedCode.SupplierCodeId,
                                              CodeGroupId = matchedCode.CodeGroupId
                                          });
                                      }
                                      else if (matchedCode.BeginEffectiveDate < code.BED)
                                      {
                                          if (matchedCode.EndEffectiveDate < code.BED)
                                          {
                                              zone.Codes.Add(new Code
                                              {
                                                  BeginEffectiveDate = code.BED,
                                                  EndEffectiveDate = code.EED,
                                                  Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                                                  CodeValue = code.Code,
                                                  ZoneId = zone.SupplierZoneId,
                                                  CodeGroupId = codeGroup.CodeGroupId
                                              });
                                          }
                                          else if (matchedCode.EndEffectiveDate > code.BED && matchedCode.EndEffectiveDate < code.EED)
                                          {
                                              zone.Codes.Add(new Code
                                              {
                                                  BeginEffectiveDate = matchedCode.BeginEffectiveDate,
                                                  EndEffectiveDate = code.BED,
                                                  Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated,
                                                  CodeValue = matchedCode.CodeValue,
                                                  ZoneId = matchedCode.ZoneId,
                                                  SupplierCodeId = matchedCode.SupplierCodeId,
                                                  CodeGroupId = matchedCode.CodeGroupId
                                              });
                                              zone.Codes.Add(new Code
                                              {
                                                  BeginEffectiveDate = code.BED,
                                                  EndEffectiveDate = code.EED,
                                                  Status = TOne.WhS.SupplierPriceList.Entities.Status.New,
                                                  CodeValue = code.Code,
                                                  ZoneId = zone.SupplierZoneId,
                                                  CodeGroupId = codeGroup.CodeGroupId
                                              });
                                          }
                                          else if (matchedCode.EndEffectiveDate > zone.EndEffectiveDate || matchedCode.EndEffectiveDate == null)
                                          {
                                              zone.Codes.Add(new Code
                                              {
                                                  BeginEffectiveDate = matchedCode.BeginEffectiveDate,
                                                  EndEffectiveDate = code.EED,
                                                  Status = TOne.WhS.SupplierPriceList.Entities.Status.Updated,
                                                  CodeValue = matchedCode.CodeValue,
                                                  ZoneId = matchedCode.ZoneId,
                                                  SupplierCodeId = matchedCode.SupplierCodeId,
                                                  CodeGroupId = matchedCode.CodeGroupId
                                              });
                                          }
                                      }
                                  }
                              }

                          }
                          

                        
                    }
                  
                
                }

            }
            List<Code> codesToBeDeleted=new List<Code>();
            foreach (var obj in codesByCode)
            {
                foreach (Code code in obj.Value)
                {
                    code.EndEffectiveDate = inputArgument.MinimumDate;
                    codesToBeDeleted.Add(code);
                }
                    
            }
            TimeSpan spent = DateTime.Now.Subtract(startPreparing);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "ProcessCodes done and takes:{0}", spent);
        }

        protected override ProcessCodesInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new ProcessCodesInput
            {
                Zones = this.Zones.Get(context),
                MinimumDate = this.MinimumDate.Get(context),
                SupplierId = this.SupplierId.Get(context),
                CodesToBeDeleted = this.CodesToBeDeleted.Get(context),
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.Zones.Get(context) == null)
                this.Zones.Set(context, new List<Zone>());
            if (this.CodesToBeDeleted.Get(context) == null)
                this.CodesToBeDeleted.Set(context, new List<Code>());

            base.OnBeforeExecute(context, handle);
        }
    }
}
