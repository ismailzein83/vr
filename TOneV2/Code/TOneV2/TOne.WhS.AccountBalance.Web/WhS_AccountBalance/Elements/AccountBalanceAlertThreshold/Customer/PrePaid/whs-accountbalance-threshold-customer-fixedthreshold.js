'use strict';

app.directive('whsAccountbalanceThresholdCustomerFixedthreshold', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomerFixedPercentageThreshold($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceAlertThreshold/Customer/PrePaid/Template/CustomerFixedPercentageThreshold.html'
        };

        function CustomerFixedPercentageThreshold($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var currencySelectorAPI;
            var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCurrencySelectorReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var threshold;
                    var currencyId;

                    if (payload != undefined) {
                        threshold = payload.Threshold;
                        currencyId = payload.CurrencyId;
                    }

                    $scope.scopeModel.threshold = threshold;

                    //Loading Currency Selector
                    var loadCurrencySelectorPromise = getLoadCurrencySelectorPromise();
                    promises.push(loadCurrencySelectorPromise);


                    function getLoadCurrencySelectorPromise() {
                        var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        currencySelectorReadyDeferred.promise.then(function () {

                            var currencySelectorPayload = {};
                            if (currencyId) {
                                currencySelectorPayload.selectedIds = currencyId;
                            }
                            else {
                                currencySelectorPayload.selectSystemCurrency = true;
                            }

                            VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencySelectorLoadDeferred);
                        });

                        return currencySelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertThresholds.PrePaid.CustFixedThreshold, TOne.WhS.AccountBalance.MainExtensions',
                        Threshold: $scope.scopeModel.threshold,
                        ThresholdDescription: $scope.scopeModel.threshold,
                        CurrencyId: currencySelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }]);