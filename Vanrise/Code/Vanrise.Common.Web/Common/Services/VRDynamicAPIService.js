app.service('VR_Dynamic_APIService', ['VRModalService', 'VRNotificationService',
    function (VRModalService, VRNotificationService) {

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
        return {
            addVRDynamicAPI: addVRDynamicAPI,
            editVRDynamicAPI: editVRDynamicAPI,
            addVRDynamicAPIMethod: addVRDynamicAPIMethod,
            editVRDynamicAPIMethod: editVRDynamicAPIMethod,
            displayErrors: displayErrors,
        };
    }]);