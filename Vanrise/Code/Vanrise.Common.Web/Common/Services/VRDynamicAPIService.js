app.service('VRCommon_DynamicAPIService', ['VRModalService', 'VRCommon_ObjectTrackingService',
    function (VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function addVRDynamicAPI(onVRDynamicAPIAdded, vrDynamicAPIModuleId) {

            var settings = {};
            var parameters = {
                vrDynamicAPIModuleId: vrDynamicAPIModuleId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRDynamicAPIAdded = onVRDynamicAPIAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRDynamicAPI/VRDynamicAPIEditor.html', parameters, settings);
        }

        function editVRDynamicAPI(vrDynamicAPIId, onVRDynamicAPIUpdated, vrDynamicAPIModuleId) {

            var settings = {};
            var parameters = {
                vrDynamicAPIId: vrDynamicAPIId,
                vrDynamicAPIModuleId: vrDynamicAPIModuleId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRDynamicAPIUpdated = onVRDynamicAPIUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRDynamicAPI/VRDynamicAPIEditor.html', parameters, settings);
        }

        function addVRDynamicAPIMethod(onVRDynamicAPIMethodAdded) {

            var settings = {};
            var parameters = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRDynamicAPIMethodAdded = onVRDynamicAPIMethodAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRDynamicAPI/VRDynamicAPIMethodEditor.html', parameters, settings);
        }

        function editVRDynamicAPIMethod(vrDynamicAPIMethodEntity, onVRDynamicAPIMethodUpdated) {

            var settings = {};
            var parameters = {
                vrDynamicAPIMethodEntity: vrDynamicAPIMethodEntity
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRDynamicAPIMethodUpdated = onVRDynamicAPIMethodUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRDynamicAPI/VRDynamicAPIMethodEditor.html', parameters, settings);
        }

        function displayErrors(errors) {

            var settings = {};
            var parameters = {
                errors: errors
            };

            settings.onScopeReady = function (modalScope) {
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRDynamicAPI/VRDynamicAPIErrorsDisplayer.html', parameters, settings);

        }

        function getEntityUniqueName() {
            return "VR_Common_VRDynamicAPI";
        }

        function registerObjectTrackingDrillDownToVRDynamicAPI() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vrDynamicAPIItem) {
                vrDynamicAPIItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: vrDynamicAPIItem.VRDynamicAPIId,
                    EntityUniqueName: getEntityUniqueName()

                };
                return vrDynamicAPIItem.objectTrackingGridAPI.load(query);
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
            addVRDynamicAPI: addVRDynamicAPI,
            editVRDynamicAPI: editVRDynamicAPI,
            addVRDynamicAPIMethod: addVRDynamicAPIMethod,
            editVRDynamicAPIMethod: editVRDynamicAPIMethod,
            displayErrors: displayErrors,
            registerObjectTrackingDrillDownToVRDynamicAPI: registerObjectTrackingDrillDownToVRDynamicAPI,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }]);