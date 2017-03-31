(function (appControllers) {

    "use strict";

    StyleDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function StyleDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addStyleDefinition(onStyleDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onStyleDefinitionAdded = onStyleDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/StyleDefinition/StyleDefinitionEditor.html', null, settings);
        };

        function editStyleDefinition(styleDefinitionId, onStyleDefinitionUpdated) {
            var settings = {};

            var parameters = {
                styleDefinitionId: styleDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onStyleDefinitionUpdated = onStyleDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/StyleDefinition/StyleDefinitionEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_StyleDefinition";
        }

        function registerObjectTrackingDrillDownToStyleDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, styleDefinitionItem) {
                styleDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: styleDefinitionItem.Entity.StyleDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return styleDefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addStyleDefinition: addStyleDefinition,
            editStyleDefinition: editStyleDefinition,
            registerObjectTrackingDrillDownToStyleDefinition: registerObjectTrackingDrillDownToStyleDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VRCommon_StyleDefinitionService', StyleDefinitionService);

})(appControllers);