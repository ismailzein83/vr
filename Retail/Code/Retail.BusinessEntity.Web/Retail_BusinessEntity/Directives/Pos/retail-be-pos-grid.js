'use strict';

app.directive('retailBePointOfSaleGrid', ['Retail_BE_PointOfSaleAPIService', 'Retail_BE_PointOfSaleService', 'VRNotificationService', function (Retail_BE_PointOfSaleAPIService, Retail_BE_PointOfSaleService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var pointOfSaleGrid = new PointOfSaleGrid($scope, ctrl, $attrs);
            pointOfSaleGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Pos/Templates/PosGridTemplate.html'
    };

    function PointOfSaleGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.pointOfSales = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_PointOfSaleAPIService.GetFilteredPointOfSales(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        }
        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onPointOfSaleAdded = function (addedPointOfSale) {
                gridAPI.itemAdded(addedPointOfSale);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editPointOfSale,
                haspermission: hasEditPointOfSalePermission
            });
        }
        function editPointOfSale(pointOfSale) {
            var onPointOfSaleUpdated = function (updatedPointOfSale) {
                gridAPI.itemUpdated(updatedPointOfSale);
            };
            Retail_BE_PointOfSaleService.editPointOfSale(pointOfSale.Entity.Id, onPointOfSaleUpdated);
        }
        function hasEditPointOfSalePermission() {
            return Retail_BE_PointOfSaleAPIService.HasUpdatePointOfSalePermission();
        }
    }
}]);
