(function (app) {

    'use strict';

    manufactoryGrid.$inject = ['Demo_Module_ManufactoryAPIService', 'Demo_Module_ManufactoryService', 'VRNotificationService', 'VRUIUtilsService'];

    function manufactoryGrid(Demo_Module_ManufactoryAPIService, Demo_Module_ManufactoryService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var ctor = new manufactoryGridCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Manufactory/Directives/Templates/ManufactoryGridTemplate.html'
        };

        function manufactoryGridCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.manufactories = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownDefinitions(), gridAPI);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_ManufactoryAPIService.GetFilteredManufactories(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineGridMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onManufactoryAdded = function (manufactory) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(manufactory);
                    gridAPI.itemAdded(manufactory);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildDrillDownDefinitions() {
                var drillDownDefinitions = [];
                drillDownDefinitions.push(buildProductDrillDownDefinition());
                return drillDownDefinitions;
            }

            function buildProductDrillDownDefinition() {
                var drillDownDefinition = {};

                drillDownDefinition.title = 'Product';
                drillDownDefinition.directive = 'demo-module-product-search';

                drillDownDefinition.loadDirective = function (directiveAPI, manufactoryItem) {
                    manufactoryItem.productAPI = directiveAPI;

                    var payload = {
                        manufactoryId: manufactoryItem.Id
                    };
                    return manufactoryItem.productAPI.load(payload);
                };

                return drillDownDefinition;
            }

            function defineGridMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: 'Edit',
                    clicked: onEditManufactory
                }];

                function onEditManufactory(manufactory) {
                    var onManufactoryUpdated = function (manufactory) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(manufactory);
                        gridAPI.itemUpdated(manufactory);
                    };

                    Demo_Module_ManufactoryService.editManufactory(onManufactoryUpdated, manufactory.Id);
                }
            }
        }
    }

    app.directive('demoModuleManufactoryGrid', manufactoryGrid);
})(app);