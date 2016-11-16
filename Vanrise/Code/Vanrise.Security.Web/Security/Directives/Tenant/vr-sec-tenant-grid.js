"use strict";

app.directive("vrSecTenantGrid", ["VR_Sec_TenantAPIService", "VR_Sec_TenantService", 'VR_Sec_PermissionAPIService', "VR_Sec_PermissionService", "VR_Sec_HolderTypeEnum", 'VRNotificationService',
    function (VR_Sec_TenantAPIService, VR_Sec_TenantService, VR_Sec_PermissionAPIService, VR_Sec_PermissionService, VR_Sec_HolderTypeEnum, VRNotificationService) {

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
        templateUrl: "/Client/Modules/Security/Directives/Tenant/Templates/TenantsGrid.html"

    };

    function TenantsGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.tenants = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onTenantAdded = function (tenantObject) {
                        gridAPI.itemAdded(tenantObject);
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_TenantAPIService.GetFilteredTenants(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editTenant,
                haspermission: hasUpdateUserPermission
            }];

            function hasUpdateUserPermission() {
                return VR_Sec_TenantAPIService.HasUpdateTenantPermission();
            }
        }

        function editTenant(tenantObj) {
            var onTenanUpdated = function (tenantObj) {
                gridAPI.itemUpdated(tenantObj);
            };

            VR_Sec_TenantService.editTenant(tenantObj.Entity.TenantId, onTenanUpdated);
        }
    }

    return directiveDefinitionObject;

}]);