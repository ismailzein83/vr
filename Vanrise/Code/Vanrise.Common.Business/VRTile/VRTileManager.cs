using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;


namespace Vanrise.Common.Business
{
    public class VRTileManager
    {
        public IEnumerable<VRTileExtendedSettingsConfig> GetTileExtendedSettingsConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRTileExtendedSettingsConfig>(VRTileExtendedSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<FiguresTileQueryDefinitionSettingsConfig> GetFiguresTilesDefinitionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<FiguresTileQueryDefinitionSettingsConfig>(FiguresTileQueryDefinitionSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<FiguresTileSchemaInfo> GetFiguresTileItemsToDiplayInfo(FiguresTileQueriesInput queriesInput)
        {
            FiguresTileQueryGetSchemaContext context = new FiguresTileQueryGetSchemaContext();
            List<FiguresTileSchemaInfo> figuresTileSchemaInfos = new List<FiguresTileSchemaInfo>();
            queriesInput.ThrowIfNull("queriesInput");
            var queries = queriesInput.Queries;
            queries.ThrowIfNull("queries");
            foreach (var query in queries)
            {
                query.Settings.ThrowIfNull("query.Settings");
                var figureItems = query.Settings.GetSchema(context);
                figureItems.ThrowIfNull("figureItems");
                foreach (var figureItem in figureItems)
                {
                    figuresTileSchemaInfos.Add(new FiguresTileSchemaInfo() {
                        QueryId = query.FiguresTileQueryId,
                        Name = figureItem.Name,
                        Title = figureItem.Title,
                        QueryName = query.Name
                    });
                }
            }
            return figuresTileSchemaInfos;
        }
        public List<FigureItemValue> GetFigureItemsValue (List<FiguresTileQuery> queries, List<FiguresTileDisplayItem> itemsToDisplay)
        {
            FiguresTileQueryExecuteContext context = new FiguresTileQueryExecuteContext();
            List<FigureItemValue> orderedfigureItemValue = new List<FigureItemValue>();
            List<FigureItemValue> queriesFigureItemsValue = new List<FigureItemValue>();
            foreach (var query in queries)
            {
                var selectedItemsToDisplay = new List<FiguresTileDisplayItem>();
                foreach(var item in itemsToDisplay)
                {
                    if (item.FiguresTileQueryId == query.FiguresTileQueryId)
                        selectedItemsToDisplay.Add(item);
                }
               
                if(selectedItemsToDisplay.Count()>0)
                {
                    context.ItemsToDisplay = selectedItemsToDisplay;
                     queriesFigureItemsValue.AddRange(query.Settings.Execute(context)) ;
                }
            }
            if(queriesFigureItemsValue.Count() >0)
            foreach(var item in itemsToDisplay)
            {
                    var figureItem = queriesFigureItemsValue.FindRecord(x=>x.Name == item.Title);
                    if (figureItem != null)
                        orderedfigureItemValue.Add(figureItem);
            }
            return orderedfigureItemValue;
        }
    }
}
