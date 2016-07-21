(function (app) {

    'use strict';

    DealGridDirective.$inject = ['WhS_BE_DealAPIService', 'WhS_BE_DealService', 'VRNotificationService'];

    function DealGridDirective(WhS_BE_DealAPIService, WhS_BE_DealService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dealGrid = new DealGrid($scope, ctrl, $attrs);
                dealGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/Deal/Templates/DealGridTemplate.html'
        };

        function DealGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_BE_DealAPIService.GetFilteredDeals(dataRetrievalInput).then(function (response) {
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

                api.onDealAdded = function (addedDeal) {
                    gridAPI.itemAdded(addedDeal);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editDeal,
                    haspermission: hasEditPermission
                }];

                function editDeal(dataItem) {
                    var onDealUpdated = function (updatedDeal) {
                        gridAPI.itemUpdated(updatedDeal);
                    };
                    WhS_BE_DealService.editDeal(dataItem.Entity.DealId, onDealUpdated);
                }

                function hasEditPermission() {
                    return WhS_BE_DealAPIService.HasEditDealPermission();
                }
            }
        }
    }

    app.directive('vrWhsBeDealGrid', DealGridDirective);

})(app);