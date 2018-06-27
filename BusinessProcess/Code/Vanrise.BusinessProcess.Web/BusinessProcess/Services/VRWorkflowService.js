(function (appControllers) {

    "use strict";
    BusinessProcess_VRWorkflowService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];
   
    function BusinessProcess_VRWorkflowService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function addWorkflow(onWorkflowAdded) {
            var modalParameters = {
       
            };
           
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onWorkflowAdded = onWorkflowAdded;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/VRWorkflow/VRWorkflowEditor.html', modalParameters, modalSettings);
        }

        function editWorkflow(vrWorkflowId, onVRWorkflowUpdated) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRWorkflowUpdated = onVRWorkflowUpdated;
            };
            var parameters = {
                vrWorkflowId: vrWorkflowId
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/VRWorkflow/VRWorkflowEditor.html', parameters, settings);
        }
        
        function getEntityUniqueName() {
            return "BusinessProcess_VR_Workflow";
        }

        function registerObjectTrackingDrillDownToVRWorkflow() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vrWorkflow) {
                vrWorkflow.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: vrWorkflow.VRWorkflowID,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return vrWorkflow.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
        return ({
            addWorkflow: addWorkflow,
            editWorkflow: editWorkflow,
            registerObjectTrackingDrillDownToVRWorkflow: registerObjectTrackingDrillDownToVRWorkflow,
            getDrillDownDefinition: getDrillDownDefinition
        });


    }
    appControllers.service('BusinessProcess_VRWorkflowService', BusinessProcess_VRWorkflowService);

})(appControllers);