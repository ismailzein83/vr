"use strict";

app.directive("vrWhsDealVolumecommitmentLastdealprogress", ["UtilsService", "VRNotificationService", "WhS_Deal_DealDefinitionAPIService",
    function (UtilsService, VRNotificationService, WhS_Deal_DealDefinitionAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dealVolumecommitmentLastdealprogres = new DealVolumecommitmentLastdealprogres($scope, ctrl, $attrs);
                dealVolumecommitmentLastdealprogres.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentLastDealProgressTemplate.html'

        };

        function DealVolumecommitmentLastdealprogres($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var costGridAPI;
            var saleGridAPI;
            var saleGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var costGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var carrierAccountId;
            var dealInfo;

            function initializeController() {
                ctrl.saleDataSource = [];
                ctrl.costDataSource = [];
                var promises = [];

                $scope.onCostGridReady = function (api) {
                    costGridAPI = api;
                    costGridReadyPromiseDeferred.resolve();
                };

                $scope.onSaleGridReady = function (api) {
                    saleGridAPI = api;
                    saleGridReadyPromiseDeferred.resolve();
                };
                if (costGridAPI != undefined)
                    promises.push(costGridReadyPromiseDeferred.promise);
                if (saleGridAPI != undefined)
                    promises.push(saleGridReadyPromiseDeferred.promise);
                return UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    dealInfo = payload.dealInfo;
                    if (dealInfo != undefined) {
                        
                        return WhS_Deal_DealDefinitionAPIService.GetLastVolumeCommitmentProgress(dealInfo.DealId).then(function (response) {
                            var volCommitmentProgress = response;
                            if (volCommitmentProgress != undefined) {
                             
                                  for (var i = 0; i < volCommitmentProgress.length; i++) {
                                      var progress = volCommitmentProgress[i];
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