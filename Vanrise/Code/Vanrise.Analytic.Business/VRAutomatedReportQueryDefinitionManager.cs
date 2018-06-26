using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
     public class VRAutomatedReportQueryDefinitionManager
    {

        #region Ctor/Properties

        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();
        #endregion

        #region Public Methods
        public IEnumerable<VRAutomatedReportQueryDefinitionSettingsConfig> GetAutomatedReportTemplateConfigs()
        {
            return BusinessManagerFactory.GetManager<IExtensionConfigurationManager>().GetExtensionConfigurations<VRAutomatedReportQueryDefinitionSettingsConfig>(VRAutomatedReportQueryDefinitionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RecordSearchQueryDefinitionInfo> GetVRAutomatedReportQueryDefinitionsInfo(RecordSearchQueryDefinitionInfoFilter filter)
        {
            Func<VRAutomatedReportQueryDefinition, bool> filterExpression = null;
            return _vrComponentTypeManager.GetComponentTypes<VRAutomatedReportQueryDefinitionSettings, VRAutomatedReportQueryDefinition>().MapRecords(RecordSearchQueryDefinitionInfoMapper, filterExpression);
        }
        public VRAutomatedReportQueryDefinitionSettings GetVRAutomatedReportQueryDefinitionSettings(Guid vrComponentTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<VRAutomatedReportQueryDefinitionSettings>(vrComponentTypeId);
        }

        public Dictionary<Guid, VRAutomatedReportDataSchema> GetAutomatedReportDataSchema(AutomatedReportQueries input)
        {
            Dictionary<Guid, VRAutomatedReportDataSchema> schema = new Dictionary<Guid, VRAutomatedReportDataSchema>();
            if (input != null)
            {
                if (input.Queries != null && input.Queries.Count > 0)
                {
                    foreach (var query in input.Queries)
                    {
                        query.Settings.ThrowIfNull("query.Settings", query.VRAutomatedReportQueryId);
                        var querySchema = query.Settings.GetSchema(new VRAutomatedReportQueryGetSchemaContext()
                        {
                            QueryDefinitionId = query.DefinitionId
                        });
                        schema.Add(query.VRAutomatedReportQueryId, querySchema);
                    }
                }
            }
            return schema;
        }

        public ValidateQueryAndHandlerSettingsResult ValidateQueryAndHandlerSettings(ValidateQueryAndHandlerSettingsInput input)
        {
            input.ThrowIfNull("No queries nor handlers were added.");
            input.Queries.ThrowIfNull("No queries were added.");
            input.HandlerSettings.ThrowIfNull("input.HandlerSettings");
            VRAutomatedReportHandlerValidateContext context = new VRAutomatedReportHandlerValidateContext{
                Queries = input.Queries
            };
            input.HandlerSettings.Validate(context);
            return new ValidateQueryAndHandlerSettingsResult()
            {
                ErrorMessage = context.ErrorMessage,
                Result = context.Result
            };
        }

        #endregion

        #region Mappers

        private RecordSearchQueryDefinitionInfo RecordSearchQueryDefinitionInfoMapper(VRAutomatedReportQueryDefinition vrAutomatedReportQueryDefinition)
        {
            string editor = null;
            if (vrAutomatedReportQueryDefinition != null && vrAutomatedReportQueryDefinition.Settings != null && vrAutomatedReportQueryDefinition.Settings.ExtendedSettings != null)
                editor = vrAutomatedReportQueryDefinition.Settings.ExtendedSettings.RuntimeEditor;
            return new RecordSearchQueryDefinitionInfo
            {
                DefinitionId = vrAutomatedReportQueryDefinition.VRComponentTypeId,
                Name = vrAutomatedReportQueryDefinition.Name,
                RuntimeEditor = editor
            };
        }
         
        #endregion



    }
}
