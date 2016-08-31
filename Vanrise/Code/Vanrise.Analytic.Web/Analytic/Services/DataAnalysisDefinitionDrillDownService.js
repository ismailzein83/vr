(function (appControllers) {

    'use stict';

    DataAnalysisGridService.$inject = ['VRModalService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_DataAnalysisItemDefinitionService'];

    function DataAnalysisGridService(VRModalService, VRNotificationService, VRUIUtilsService, VR_Analytic_DataAnalysisItemDefinitionService) {

        function defineDataAnalysisItemDefinitionTabsAndMenuActions(dataAnalysisDefinition, gridAPI, criteriaFieldValues) {
            if (dataAnalysisDefinition.Entity.Settings == null || dataAnalysisDefinition.Entity.Settings.ItemsConfig == null)
                return;

            var drillDownTabs = [];
            var menuActions = [];

            for (var i = 0; i < dataAnalysisDefinition.Entity.Settings.ItemsConfig.length; i++) {
                var dataAnalysisItemDefinitionConfig = dataAnalysisDefinition.Entity.Settings.ItemsConfig[i];

                addDrillDownTab(dataAnalysisItemDefinitionConfig);
                addMenuAction(dataAnalysisItemDefinitionConfig, i);
            }

            setDrillDownTabs();
            setMenuActions();


            function addDrillDownTab(dataAnalysisItemDefinitionConfig) {
                var drillDownTab = {};

                drillDownTab.title = dataAnalysisItemDefinitionConfig.Title;
                drillDownTab.directive = dataAnalysisItemDefinitionConfig.GridDirective;

                drillDownTab.loadDirective = function (dataAnalysisItemDefinitionGridAPI, dataAnalysisDefinition) {
                    dataAnalysisDefinition.dataAnalysisItemDefinitionGridAPI = dataAnalysisItemDefinitionGridAPI;

                    return dataAnalysisDefinition.dataAnalysisItemDefinitionGridAPI.load(buildDataAnalysisItemDefinitionQuery());
                };

                function buildDataAnalysisItemDefinitionQuery() {

                    var dataAnalysisItemDefinitionQuery = {};

                    dataAnalysisItemDefinitionQuery.DataAnalysisDefinitionId = dataAnalysisDefinition.Entity.DataAnalysisDefinitionId;
                    dataAnalysisItemDefinitionQuery.ItemDefinitionTypeId = dataAnalysisDefinition.Entity.Settings.ItemDefinitionTypeId;

                    return dataAnalysisItemDefinitionQuery;
                }

                drillDownTabs.push(drillDownTab);
            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI, undefined);
                drillDownManager.setDrillDownExtensionObject(dataAnalysisDefinition);  
            }

            function addMenuAction(dataAnalysisItemDefinitionConfig, dataAnalysisItemDefinitionConfigIndex) {
                var menuAction = {};

                menuAction.name = 'New ' + dataAnalysisItemDefinitionConfig.Title;
                menuAction.clicked = function (dataAnalysisDefinition) {

                    dataAnalysisDefinition.drillDownExtensionObject.drillDownDirectiveTabs[dataAnalysisItemDefinitionConfigIndex].setTabSelected(dataAnalysisDefinition);

                    var onDataAnalysisItemDefinitionAdded = function (addedDataAnalysisItemDefinition) {
                        dataAnalysisDefinition.dataAnalysisItemDefinitionGridAPI.onItemAdded(addedDataAnalysisItemDefinition);
                    };

                    VR_Analytic_DataAnalysisItemDefinitionService.addDataAnalysisItemDefinition(dataAnalysisDefinition.Entity.DataAnalysisDefinitionId, onDataAnalysisItemDefinitionAdded);
                };

                menuActions.push(menuAction);
            }
            function setMenuActions() {
                dataAnalysisDefinition.menuActions = [];
                for (var i = 0; i < menuActions.length; i++) {
                    dataAnalysisDefinition.menuActions.push(menuActions[i]);
                }
            }
        }

        return {
            defineDataAnalysisItemDefinitionTabsAndMenuActions: defineDataAnalysisItemDefinitionTabsAndMenuActions
        };
    }

    appControllers.service('VR_Analytic_DataAnalysisDefinitionDrillDownService', DataAnalysisGridService);

})(appControllers);