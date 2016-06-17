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

        this.initializeController = initializeController;

        var gridAPI;
        var drillDownManager;

        function initializeController() {

            $scope.packages = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI, $scope.gridMenuActions);
                defineAPI();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_PackageAPIService.GetFilteredPackages(dataRetrievalInput).then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++)
                            drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        }
        function defineAPI()
        {
            var api = {};

            api.loadGrid = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onPackageAdded = function (packageObject) {
                drillDownManager.setDrillDownExtensionObject(packageObject);
                gridAPI.itemAdded(packageObject);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        
        function buildDrillDownTabs() {
            var drillDownTabs = [];

            var servicesTab = buildServicesTab();
            drillDownTabs.push(servicesTab);

            function buildServicesTab() {
                return {
                    title: 'Services',
                    directive: 'retail-be-package-packageservice-grid',
                    loadDirective: function (directiveAPI, dataItem) {
                        dataItem.packageServiceGridAPI = directiveAPI;
                        var packageServiceGridQuery = { PackageId: dataItem.Entity.PackageId };
                        directiveAPI.loadGrid(packageServiceGridQuery);
                    }
                };
            }

            return drillDownTabs;
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editPackage,
                haspermission: hasUpdatePackagePermission
            }];
        }
        function editPackage(packageObj) {
            var onPackageUpdated = function (packageObject) {
                drillDownManager.setDrillDownExtensionObject(packageObject);
                gridAPI.itemUpdated(packageObject);
            };
            Retail_BE_PackageService.editPackage(packageObj.Entity.PackageId, onPackageUpdated);
        }
        function hasUpdatePackagePermission() {
            return Retail_BE_PackageAPIService.HasUpdatePackagePermission();
        }
    }

    return directiveDefinitionObject;

}]);