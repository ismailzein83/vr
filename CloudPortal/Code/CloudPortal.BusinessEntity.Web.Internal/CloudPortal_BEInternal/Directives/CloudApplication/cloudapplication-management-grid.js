"use strict";

app.directive("cloudportalBeinternalCloudapplicationManagementGrid", ["UtilsService", "VRNotificationService", "CloudPortal_BEInternal_CloudApplicationService", "CloudPortal_BEInternal_CloudApplicationAPIService",
function (UtilsService, VRNotificationService, CloudPortal_BEInternal_CloudApplicationService, CloudPortal_BEInternal_CloudApplicationAPIService) {

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

        this.initializeController = initializeController;

        function initializeController() {

            $scope.cloudApplications = [];
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

                    directiveAPI.onCloudApplicationAdded = function (cloudApplicationObj) {
                        gridAPI.itemAdded(cloudApplicationObj);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CloudPortal_BEInternal_CloudApplicationAPIService.GetFilteredCloudApplications(dataRetrievalInput)
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
             { name: "Edit", clicked: editCloudApplication }
            ];
        }

        function editCloudApplication(cloudApplication) {

            var onCloudApplicationUpdated = function (updatedItem) {
                var updatedItemObj = { Entity: updatedItem };
                gridAPI.itemUpdated(updatedItemObj);
            };

            CloudPortal_BEInternal_CloudApplicationService.editCloudApplication(cloudApplication.Entity.CloudApplicationId, onCloudApplicationUpdated);
        };
    }

    return directiveDefinitionObject;

}]);