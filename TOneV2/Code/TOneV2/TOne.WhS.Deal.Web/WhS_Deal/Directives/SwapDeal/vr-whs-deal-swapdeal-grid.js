(function (app) {

    'use strict';

    SwapDealGridDirective.$inject = ['WhS_Deal_DealAPIService', 'WhS_Deal_SwapDealService', 'VRNotificationService'];

    function SwapDealGridDirective(WhS_Deal_DealAPIService, WhS_Deal_SwapDealService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var swapDealGrid = new SwapDealGrid($scope, ctrl, $attrs);
                swapDealGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/swapDeal/Templates/SwapDealGridTemplate.html'
        };

        function SwapDealGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.swapDeals = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Deal_DealAPIService.GetFilteredSwapDeals(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onSwapDealAdded = function (addedDeal) {
                    gridAPI.itemAdded(addedDeal);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editDeal
                }];

                function editDeal(dataItem) {
                    var onDealUpdated = function (updatedDeal) {
                        gridAPI.itemUpdated(updatedDeal);
                    };
                    WhS_Deal_SwapDealService.editSwapDeal(dataItem.Entity.DealId, onDealUpdated);
                }
            }
        }
    }

    app.directive('vrWhsDealSwapdealGrid', SwapDealGridDirective);

})(app);