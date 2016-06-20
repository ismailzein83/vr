'use strict';

app.directive('retailBePackagePackageserviceGrid', ['Retail_BE_PackageAPIService', 'Retail_BE_ServiceTypeService', 'UtilsService', 'VRNotificationService', function (Retail_BE_PackageAPIService, Retail_BE_ServiceTypeService, UtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var packageServiceGrid = new PackageServiceGrid($scope, ctrl, $attrs);
            packageServiceGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageItem/Templates/PackageServiceGridTemplate.html'
    };

    function PackageServiceGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        var packageId;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.packageServices = [];

            $scope.scopeModel.menuActions = function (dataItem) {
                var menuActions = [];
                if (dataItem.menuActions != null) {
                    for (var i = 0; i < dataItem.menuActions.length; i++)
                        menuActions.push(dataItem.menuActions[i]);
                }
                return menuActions;
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_PackageAPIService.GetFilteredPackageServices(dataRetrievalInput).then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var dataItem = response.Data[i];
                            Retail_BE_ServiceTypeService.defineServiceTypeRuleTabsAndMenuActions(dataItem, gridAPI, buildCriteriaFieldValues(dataItem));
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            $scope.scopeModel.showExpandIcon = function (dataItem) {
                return (dataItem.drillDownExtensionObject.drillDownDirectiveTabs.length > 0);
            };
        }
        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                packageId = query.PackageId;
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function buildCriteriaFieldValues(dataItem) {
            return {
                'Package': packageId,
                'ServiceType': dataItem.Entity.ServiceTypeId
            };
        }
    }
}]);
