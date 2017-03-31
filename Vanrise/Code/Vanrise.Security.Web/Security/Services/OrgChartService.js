(function (appControllers) {

    'use strict';

    OrgChartService.$inject = ['VR_Sec_OrgChartAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];
    var drillDownDefinitions = [];
    function OrgChartService(VR_Sec_OrgChartAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        return {
            addOrgChart: addOrgChart,
            editOrgChart: editOrgChart,
            deleteOrgChart: deleteOrgChart,
            registerObjectTrackingDrillDownToOrgChart: registerObjectTrackingDrillDownToOrgChart,
            getDrillDownDefinition: getDrillDownDefinition
        };

        function addOrgChart(onOrgChartAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOrgChartAdded = onOrgChartAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/OrgChart/OrgChartEditor.html', null, modalSettings);
        }

        function editOrgChart(orgChartId, onOrgChartUpdated) {
            var modalParameters = {
                orgChartId: orgChartId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOrgChartUpdated = onOrgChartUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/OrgChart/OrgChartEditor.html', modalParameters, modalSettings);
        }

        // scope is the grid's $scope
        function deleteOrgChart(scope, orgChartId, onOrgChartDeleted) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    VR_Sec_OrgChartAPIService.DeleteOrgChart(orgChartId).then(function (response) {
                        if (response) {
                            var deleted = VRNotificationService.notifyOnItemDeleted('Org Chart', response);

                            if (deleted && onOrgChartDeleted && typeof onOrgChartDeleted == 'function') {
                                onOrgChartDeleted();
                            }
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }

        function getEntityUniqueName() {
            return "VR_Security_OrgChart";
        }

        function registerObjectTrackingDrillDownToOrgChart() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, orgChartItem) {
                orgChartItem.objectTrackingGridAPI = directiveAPI;
                
                var query = {
                    ObjectId: orgChartItem.OrgChartId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return orgChartItem.objectTrackingGridAPI.load(query);
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

    appControllers.service('VR_Sec_OrgChartService', OrgChartService);

})(appControllers);
