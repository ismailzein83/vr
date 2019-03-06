using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.Business
{
    public class JazzReportDefinitionManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public static Guid _definitionId = new Guid("3B002607-9366-4783-8C4B-99DFE7884932");

    

        public List<JazzReportDefinition> GetAllReportDefinitions()
        {
            return GetCachedReportDefinitions().Values.ToList();
        }

        public string GetJazzReportDefinitionName(Guid reportId)
        {
            return GetJazzReportDefinitionById(reportId).Name;

        }

        public JazzReportDefinition GetJazzReportDefinitionById(Guid reportId)
        {
            var reports = GetCachedReportDefinitions();
            return reports.GetRecord(reportId);
        }
        private Dictionary<Guid, JazzReportDefinition> GetCachedReportDefinitions()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedReportDefinitions", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, JazzReportDefinition> result = new Dictionary<Guid, JazzReportDefinition>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        JazzReportDefinition reportDefintion = new JazzReportDefinition()
                        {
                            JazzReportDefinitionId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Direction = (ReportDefinitionDirection)genericBusinessEntity.FieldValues.GetRecord("Direction"),
                            SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
                            IsEnabled = (bool)genericBusinessEntity.FieldValues.GetRecord("IsEnabled")
                        };

                        var taxOption = genericBusinessEntity.FieldValues.GetRecord("TaxOption");
                        if (taxOption != null)
                            reportDefintion.TaxOption = (TaxOption)taxOption;
                        var amountMeasureType = genericBusinessEntity.FieldValues.GetRecord("AmountMeasureType");
                        if (amountMeasureType != null)
                            reportDefintion.AmountMeasureType = (AmountMeasureType)amountMeasureType;
                        var amountType = genericBusinessEntity.FieldValues.GetRecord("AmountType");
                        if (amountType != null)
                            reportDefintion.AmountType = (AmountType)amountType;
                        var splitRateValue = genericBusinessEntity.FieldValues.GetRecord("SplitRateValue");
                        if (splitRateValue != null)
                            reportDefintion.SplitRateValue = (decimal)splitRateValue;
                        var currencyId = genericBusinessEntity.FieldValues.GetRecord("CurrencyId");
                        if (currencyId != null)
                            reportDefintion.CurrencyId = (int)currencyId;

                        reportDefintion.Settings = (JazzReportDefinitionSettings)genericBusinessEntity.FieldValues.GetRecord("Settings");
                        result.Add(reportDefintion.JazzReportDefinitionId, reportDefintion);
                    }
                }

                return result;
            });
        }

        public void ValidateJazzReportDefinition(GenericBusinessEntity genericBusinessEntity, HandlerOperationType operationType, OutputResult outputResult)
        {
            JazzReportDefinition jazzReportDefinition = new JazzReportDefinition
            {
                Direction = (ReportDefinitionDirection)genericBusinessEntity.FieldValues.GetRecord("Direction"),
            };
            var taxOption = genericBusinessEntity.FieldValues.GetRecord("TaxOption");
            if (taxOption != null)
                jazzReportDefinition.TaxOption = (TaxOption)taxOption;

            var amountMeasureType = genericBusinessEntity.FieldValues.GetRecord("AmountMeasureType");
            if (amountMeasureType != null)
                jazzReportDefinition.AmountMeasureType = (AmountMeasureType)amountMeasureType;

            if (jazzReportDefinition.Direction == ReportDefinitionDirection.Out)
            {
                if (jazzReportDefinition.AmountMeasureType.HasValue && jazzReportDefinition.AmountMeasureType.Value == AmountMeasureType.AMT)
                {
                    outputResult.Result = false;
                    outputResult.Messages.Add("Cannot Choose AMT Amount Measure Type With Direction Out");
                }

                if (jazzReportDefinition.TaxOption.HasValue && jazzReportDefinition.TaxOption.Value == TaxOption.TaxMeasure)
                {
                    outputResult.Result = false;
                    outputResult.Messages.Add("Suppliers Cannot Be Assigned Taxes!");
                }
            }
        }

    }

}


