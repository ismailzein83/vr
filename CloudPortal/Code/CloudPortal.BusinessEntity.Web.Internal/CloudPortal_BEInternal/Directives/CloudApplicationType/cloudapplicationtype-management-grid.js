"use strict";

app.directive("cloudportalBeinternalCloudapplicationtypeManagementGrid", ["UtilsService", "VRNotificationService", "CloudPortal_BEInternal_CloudApplicationTypeService", "CloudPortal_BEInternal_CloudApplicationTypeAPIService",
function (UtilsService, VRNotificationService, CloudPortal_BEInternal_CloudApplicationTypeService, CloudPortal_BEInternal_CloudApplicationTypeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var cloudApplicationTypeManagementGrid = new CloudApplicationTypeManagementGrid($scope, ctrl, $attrs);
            cloudApplicationTypeManagementGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/CloudPortal_BEInternal/Directives/CloudApplicationType/Templates/CloudApplicationTypeGridTemplate.html"

    };

    function CloudApplicationTypeManagementGrid($scope, ctrl, $attrs) {

        var gridAPI;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.cloudApplicationTypes = [];
            defineMenuActions();

            $scope.onGridReady = function (api) {
                gridAPI = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onCloudApplicationTypeAdded = function (cloudApplicationTypeObj) {
                        gridAPI.itemAdded(cloudApplicationTypeObj);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CloudPortal_BEInternal_CloudApplicationTypeAPIService.GetFilteredCloudApplicationTypes(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }


        function defineMenuActions() {
            $scope.gridMenuActions = [
             { name: "Edit", clicked: editCloudApplicationType }
            ];
        }

        function editCloudApplicationType(cloudApplicationType) {

            var onCloudApplicationTypeUpdated = function (updatedItem) {
                var updatedItemObj = { Entity: updatedItem };
                gridAPI.itemUpdated(updatedItemObj);
            };

            CloudPortal_BEInternal_CloudApplicationTypeService.editCloudApplicationType(cloudApplicationType.Entity.CloudApplicationTypeId, onCloudApplicationTypeUpdated);
        };
    }

    return directiveDefinitionObject;

}]);