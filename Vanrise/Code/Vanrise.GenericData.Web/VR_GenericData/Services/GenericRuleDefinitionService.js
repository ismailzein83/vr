(function (appControllers) {

    'use strict';

    GenericRuleDefinitionService.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function GenericRuleDefinitionService(VR_GenericData_GenericRuleDefinitionAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return {
            addGenericRuleDefinition: addGenericRuleDefinition,
            editGenericRuleDefinition: editGenericRuleDefinition,
            registerObjectTrackingDrillDownToGenericRuleDefinition: registerObjectTrackingDrillDownToGenericRuleDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };

        function addGenericRuleDefinition(onGenericRuleDefinitionAdded, settingsTypeName) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionAdded = onGenericRuleDefinitionAdded;
            };
            
            var parameters;
            if (settingsTypeName != undefined)
            {
                parameters = {
                    SettingsTypeName: settingsTypeName
                };
            }

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html', parameters, modalSettings);
        }

        function editGenericRuleDefinition(genericRuleDefinitionId, onGenericRuleDefinitionUpdated) {
            var modalParameters = {
                GenericRuleDefinitionId: genericRuleDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionUpdated = onGenericRuleDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html', modalParameters, modalSettings);
        }
        function getEntityUniqueName() {
            return "VR_GenericData_GenericRuleDefinition";
        }

        function registerObjectTrackingDrillDownToGenericRuleDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, genericRuleDefinitionItem) {
                
                genericRuleDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: genericRuleDefinitionItem.GenericRuleDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return genericRuleDefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionService', GenericRuleDefinitionService);

})(appControllers);