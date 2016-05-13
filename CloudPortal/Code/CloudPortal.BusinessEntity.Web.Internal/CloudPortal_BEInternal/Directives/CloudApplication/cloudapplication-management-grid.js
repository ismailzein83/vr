"use strict";

app.directive("cloudportalBeinternalCloudapplicationManagementGrid", ["UtilsService", "VRNotificationService", "CloudPortal_BEInternal_CloudApplicationService", "CloudPortal_BEInternal_CloudApplicationAPIService", "VRUIUtilsService", "CloudPortal_BEInternal_CloudApplicationTenantService", "CloudPortal_BEInternal_CloudApplicationTenantAPIService",
function (UtilsService, VRNotificationService, CloudPortal_BEInternal_CloudApplicationService, CloudPortal_BEInternal_CloudApplicationAPIService, VRUIUtilsService, CloudPortal_BEInternal_CloudApplicationTenantService, CloudPortal_BEInternal_CloudApplicationTenantAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var cloudApplicationManagementGrid = new CloudApplicationManagementGrid($scope, ctrl, $attrs);
            cloudApplicationManagementGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/CloudPortal_BEInternal/Directives/CloudApplication/Templates/CloudApplicationGridTemplate.html"

    };

    function CloudApplicationManagementGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.cloudApplications = [];
            defineMenuActions();

            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Assigned Tenants";
                drillDownDefinition.directive = "cloudportal-beinternal-cloudapplication-tenant-management-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, cloudApplicationItem) {

                    cloudApplicationItem.tenantGridAPI = directiveAPI;
                    var payload = {
                        ApplicationId: cloudApplicationItem.Entity.CloudApplicationId
                    };
                    return cloudApplicationItem.tenantGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(drillDownDefinition);

                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onCloudApplicationAdded = function (cloudApplicationObj) {
                        gridAPI.itemAdded(cloudApplicationObj);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CloudPortal_BEInternal_CloudApplicationAPIService.GetFilteredCloudApplications(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }


        function defineMenuActions() {
            $scope.gridMenuActions = [
             { name: "Edit", clicked: editCloudApplication, haspermission: hasUpdateCloudApplicationPermission },
             { name: "Assign Tenant", clicked: assignTenantToCloudApplication, haspermission: hasAssignCloudApplicationTenantPermission }
            ];
        }

        function hasUpdateCloudApplicationPermission() {
            return CloudPortal_BEInternal_CloudApplicationAPIService.HasUpdateCloudApplicationPermission();
        }

        function hasAssignCloudApplicationTenantPermission() {
            return CloudPortal_BEInternal_CloudApplicationTenantAPIService.HasAssignCloudApplicationTenantPermission();
        }

        function editCloudApplication(cloudApplication) {

            var onCloudApplicationUpdated = function (updatedItem) {
                var updatedItemObj = { Entity: updatedItem };
                gridAPI.itemUpdated(updatedItemObj);
            };

            CloudPortal_BEInternal_CloudApplicationService.editCloudApplication(cloudApplication.Entity.CloudApplicationId, onCloudApplicationUpdated);
        };

        function assignTenantToCloudApplication(cloudApplication) {

            var onTenantAssignedToCloudApplication = function (tenants) {

            };

            CloudPortal_BEInternal_CloudApplicationTenantService.assignTenantToCloudApplication(cloudApplication.Entity.CloudApplicationId, onTenantAssignedToCloudApplication);
        };
    }

    return directiveDefinitionObject;

}]);