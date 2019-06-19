"use strict";

app.directive("vrWhsDealSwapdealLastdealprogress", ["UtilsService", "VRNotificationService", "WhS_Deal_DealDefinitionAPIService",
    function (UtilsService, VRNotificationService, WhS_Deal_DealDefinitionAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var swapdealLastdealprogress = new SwapdealLastdealprogress($scope, ctrl, $attrs);
                swapdealLastdealprogress.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDeal/Templates/SwapDealLastDealProgressTemplate.html'

        };

        function SwapdealLastdealprogress($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var costGridAPI;
            var saleGridAPI;
            var saleGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var costGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var carrierAccountId;
            var dealInfo;

            function initializeController() {
                var promises = [];
                ctrl.saleDataSource = [];
                ctrl.costDataSource = [];


                $scope.onCostGridReady = function (api) {
                    costGridAPI = api;
                    promises.push(costGridReadyPromiseDeferred);
                    costGridReadyPromiseDeferred.resolve();
                };

                $scope.onSaleGridReady = function (api) {
                    saleGridAPI = api;
                    promises.push(saleGridReadyPromiseDeferred);
                    saleGridReadyPromiseDeferred.resolve();
                };
                return UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    dealInfo = payload.dealInfo;
                    if (dealInfo != undefined) {

                        return WhS_Deal_DealDefinitionAPIService.GetLastSwapDealProgress(dealInfo.DealId).then(function (response) {
                            var swapDealProgress = response;
                            console.log(response);
                            if (swapDealProgress != undefined) {
                                for (var i = 0; i < swapDealProgress.length; i++) {
                                    var progress = swapDealProgress[i];
                                    if (progress.IsSale)
                                        ctrl.saleDataSource.push(progress);
                                    else
                                        ctrl.costDataSource.push(progress);
                                }
                            }
                        });
                    }
                };

                api.getData = function () {

                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }


        }

        return directiveDefinitionObject;
    }
]);