(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionService.$inject = ['VRModalService', 'VRNotificationService', 'VRUIUtilsService', 'VRCommon_ObjectTrackingService'];

    function BusinessEntityDefinitionService(VRModalService, VRNotificationService, VRUIUtilsService, VRCommon_ObjectTrackingService) {
        return {
            editBusinessEntityDefinition: editBusinessEntityDefinition,
            addBusinessEntityDefinition: addBusinessEntityDefinition,
            defineBEDefinitionTabs: defineBEDefinitionTabs
        };

        function editBusinessEntityDefinition(businessEntityDefinitionId, onBusinessEntityDefinitionUpdated) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityDefinitionUpdated = onBusinessEntityDefinitionUpdated;
            };

            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/BusinessEntityDefinitionEditor/BusinessEntityDefinitionEditor.html', parameters, modalSettings);
        }
        function addBusinessEntityDefinition(onBusinessEntityDefinitionAdded) {
            var modalParameters;

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityDefinitionAdded = onBusinessEntityDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/BusinessEntityDefinitionEditor/BusinessEntityDefinitionEditor.html', modalParameters, modalSettings);
        }

        function getEntityUniqueName() {
            return "VR_GenericData_BusinessEntityDefinition";
        }
        function defineBEDefinitionTabs(beDefinitionItem, gridAPI) {
            if (beDefinitionItem == undefined)
                return;

            var drillDownTabs = [];

            if (beDefinitionItem.IsExtensible) {
                addDrillDownExtensibleTypesTab();
            }
            addDrillDownObjectTrackingTab();
            setDrillDownTabs();

            function addDrillDownExtensibleTypesTab() {
                var drillDownDefinition = {};

                drillDownDefinition.title = "Extensible Types";
                drillDownDefinition.directive = "vr-genericdata-extensiblebeitem-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, beDefinitionItem) {
                    beDefinitionItem.genericEditorGridAPI = directiveAPI;
                    var payload = {
                        BusinessEntityDefinitionId: beDefinitionItem.Entity.BusinessEntityDefinitionId
                    };
                    return beDefinitionItem.genericEditorGridAPI.loadGrid(payload);
                };
                drillDownTabs.push(drillDownDefinition);
            }

            function addDrillDownObjectTrackingTab() {

                var drillDownObjectTrackingDefinition = {};

                drillDownObjectTrackingDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
                drillDownObjectTrackingDefinition.directive = "vr-common-objecttracking-grid";

                drillDownObjectTrackingDefinition.loadDirective = function (directiveAPI, beDefinitionItem) {

                    beDefinitionItem.objectTrackingGridAPI = directiveAPI;
                    var query = {
                        ObjectId: beDefinitionItem.Entity.BusinessEntityDefinitionId,
                        EntityUniqueName: getEntityUniqueName(),

                    };
                    return beDefinitionItem.objectTrackingGridAPI.load(query);
                };

                drillDownTabs.push(drillDownObjectTrackingDefinition);
            }

            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                drillDownManager.setDrillDownExtensionObject(beDefinitionItem);
            }
        }
    }

    appControllers.service('VR_GenericData_BusinessEntityDefinitionService', BusinessEntityDefinitionService);

})(appControllers);