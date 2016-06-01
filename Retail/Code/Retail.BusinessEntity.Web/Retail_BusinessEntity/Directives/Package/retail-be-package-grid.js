"use strict";

app.directive("retailBePackageGrid", ["UtilsService", "VRNotificationService", "Retail_BE_PackageAPIService", 
    "Retail_BE_PackageService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Retail_BE_PackageAPIService, Retail_BE_PackageService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var packageGrid = new PackageGrid($scope, ctrl, $attrs);
            packageGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Package/Templates/PackageGridTemplate.html"

    };

    function PackageGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.packages = [];
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
                    directiveAPI.onPackageAdded = function (packageObject) {
                        gridAPI.itemAdded(packageObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_PackageAPIService.GetFilteredPackages(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editPackage,
                haspermission: hasUpdatePackagePermission
            }];
        }

        function hasUpdatePackagePermission() {
            return WhS_BE_PackageAPIService.HasUpdatePackagePermission();
        }

        function editPackage(packageObj) {
            var onPackageUpdated = function (packageObject) {
                gridAPI.itemUpdated(packageObject);

            }
            Retail_BE_PackageService.editPackage(packageObj.Entity.PackageId, onPackageUpdated);
        }

    }

    return directiveDefinitionObject;

}]);