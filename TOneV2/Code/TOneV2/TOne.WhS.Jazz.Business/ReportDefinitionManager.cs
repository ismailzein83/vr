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
            var records = GetCachedReportDefinitions();
            List<JazzReportDefinition> jazzReportDefinitions = null;

            if (records != null && records.Count > 0)
            {
                jazzReportDefinitions = new List<JazzReportDefinition>();
                foreach (var record in records)
                {
                    jazzReportDefinitions.Add(record.Value);
                }
            }
            return jazzReportDefinitions;
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
                            Direction = (ReportDefinitionDirectionEnum)genericBusinessEntity.FieldValues.GetRecord("Direction"),
                            SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
                            IsEnabled = (bool)genericBusinessEntity.FieldValues.GetRecord("IsEnabled")
                        };
                        reportDefintion.Direction = new ReportDefinitionDirectionEnum();
                        reportDefintion.Settings = new JazzReportDefinitionSettings();
                        reportDefintion.Settings = (JazzReportDefinitionSettings)genericBusinessEntity.FieldValues.GetRecord("Settings");
                        result.Add(reportDefintion.JazzReportDefinitionId, reportDefintion);
                    }
                }

                return result;
            });
        }
      


    }

}


