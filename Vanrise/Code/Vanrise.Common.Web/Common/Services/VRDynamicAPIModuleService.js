app.service('VR_Dynamic_API_ModuleService', ['VRModalService', 'VRNotificationService',
function (VRModalService, VRNotificationService) {

    function addVRDynamicAPIModule(onVRDynamicAPIModuleAdded) {

        var settings = {};
        var parameters = {
            
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onVRDynamicAPIModuleAdded = onVRDynamicAPIModuleAdded;
        };
        VRModalService.showModal('/Client/Modules/Common/Views/VRDynamicAPIModule/VRDynamicAPIModuleEditor.html', parameters, settings);
    }

    function editVRDynamicAPIModule(vrDynamicAPIModuleId, onVRDynamicAPIModuleUpdated) {
        var settings = {};
        var parameters = {
            vrDynamicAPIModuleId: vrDynamicAPIModuleId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onVRDynamicAPIModuleUpdated = onVRDynamicAPIModuleUpdated;

        };
        VRModalService.showModal('/Client/Modules/Common/Views/VRDynamicAPIModule/VRDynamicAPIModuleEditor.html', parameters, settings);
    }


    return {
        addVRDynamicAPIModule: addVRDynamicAPIModule,
        editVRDynamicAPIModule: editVRDynamicAPIModule,
    };

}]);