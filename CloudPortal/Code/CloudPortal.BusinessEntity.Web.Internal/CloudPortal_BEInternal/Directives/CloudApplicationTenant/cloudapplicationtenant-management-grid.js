"use strict";

app.directive("cloudportalBeinternalCloudapplicationTenantManagementGrid", ["CloudPortal_BEInternal_CloudApplicationTenantAPIService", 'VRNotificationService', 'CloudPortal_BEInternal_CloudApplicationUserService', 'VRUIUtilsService', 'CloudPortal_BEInternal_CloudApplicationTenantService', 'CloudPortal_BEInternal_CloudApplicationUserAPIService',
function (CloudPortal_BEInternal_CloudApplicationTenantAPIService, VRNotificationService, CloudPortal_BEInternal_CloudApplicationUserService, VRUIUtilsService, CloudPortal_BEInternal_CloudApplicationTenantService, CloudPortal_BEInternal_CloudApplicationUserAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var tenantGrid = new TenantsGrid($scope, ctrl, $attrs);
            tenantGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/CloudPortal_BEInternal/Directives/CloudApplicationTenant/Templates/CloudApplicationTenantsGrid.html"

    };

    function TenantsGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.tenants = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Assigned Users";
                drillDownDefinition.directive = "cloudportal-beinternal-cloudapplication-user-management-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, cloudApplicationTenantItem) {

                    cloudApplicationTenantItem.userGridAPI = directiveAPI;
                    var payload = {
                        CloudApplicationTenantId: cloudApplicationTenantItem.Entity.CloudApplicationTenantId
                    };
                    return cloudApplicationTenantItem.userGridAPI.loadGrid(payload);
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

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CloudPortal_BEInternal_CloudApplicationTenantAPIService.GetFilteredCloudApplicationTenants(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [
                { name: "Edit", clicked: editCloudApplicationTenant, haspermission: hasUpdateCloudApplicationTenantPermission },
                { name: "Assign Users", clicked: assignUsers, haspermission: hasAssignCloudApplicationUserPermission }
            ];
        }

        function hasUpdateCloudApplicationTenantPermission() {
            return CloudPortal_BEInternal_CloudApplicationTenantAPIService.HasUpdateCloudApplicationTenantPermission();
        }

        function hasAssignCloudApplicationUserPermission() {
            return CloudPortal_BEInternal_CloudApplicationUserAPIService.HasAssignCloudApplicationUserPermission();
        }


        function editCloudApplicationTenant(cloudApplicationTenant) {
            CloudPortal_BEInternal_CloudApplicationTenantService.editCloudApplicationTenant(cloudApplicationTenant.Entity.CloudApplicationTenantId);
        }

        function assignUsers(cloudApplicationTenant) {
            var onUserAssignedToCloudApplication = function (cloudApplicationUserObj) {

            };

            CloudPortal_BEInternal_CloudApplicationUserService.assignUserToCloudApplication(cloudApplicationTenant.Entity.CloudApplicationTenantId, cloudApplicationTenant.Entity.TenantId, onUserAssignedToCloudApplication);
        }
    }

    return directiveDefinitionObject;

}]);