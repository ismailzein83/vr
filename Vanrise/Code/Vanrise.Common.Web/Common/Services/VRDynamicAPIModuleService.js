app.service('VRCommon_DynamicAPIModuleService', ['VRModalService','VRCommon_ObjectTrackingService',
    function (VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

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

    function getEntityUniqueName() {
        return "VR_Common_VRDynamicAPIModule";
    }

    function registerObjectTrackingDrillDownToVRDynamicAPIModule() {
        var drillDownDefinition = {};

        drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
        drillDownDefinition.directive = "vr-common-objecttracking-grid";


        drillDownDefinition.loadDirective = function (directiveAPI, vrDynamicAPIModuleItem) {
            vrDynamicAPIModuleItem.objectTrackingGridAPI = directiveAPI;
            var query = {
                ObjectId: vrDynamicAPIModuleItem.VRDynamicAPIModuleId,
                EntityUniqueName: getEntityUniqueName()
            };
            return vrDynamicAPIModuleItem.objectTrackingGridAPI.load(query);
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
        addVRDynamicAPIModule: addVRDynamicAPIModule,
        editVRDynamicAPIModule: editVRDynamicAPIModule,
        registerObjectTrackingDrillDownToVRDynamicAPIModule: registerObjectTrackingDrillDownToVRDynamicAPIModule,
        getDrillDownDefinition: getDrillDownDefinition
    };

}]);