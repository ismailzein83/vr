'use strict';

app.directive('vrWhsDealWithcarrieraccountSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new dealCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Deal/Directives/DealDefinition/Templates/DealWithCarrierAccountSelectorTemplate.html';
            }
        };

        function dealCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var dealDefinitionSelectorAPI;
            var dealDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCarrierAccountSelectionChanged = function () {
                    var carrierId = carrierAccountDirectiveAPI.getSelectedIds();
                    if (carrierId != undefined) {
                        var dealDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        var dealDefinitionSelectorPayload = {
                            filter: {
                                $type: "TOne.WhS.Deal.MainExtensions.SwapDeal.SwapDealCarrierFilter, TOne.WhS.Deal.MainExtensions",
                                CarrierId: carrierAccountDirectiveAPI.getSelectedIds()
                            }
                        };

                        VRUIUtilsService.callDirectiveLoad(dealDefinitionSelectorAPI, dealDefinitionSelectorPayload, dealDefinitionSelectorReadyPromiseDeferred);
                    }
                    else {
                        dealDefinitionSelectorAPI.clearDataSource();

                    }
                };
                $scope.scopeModel.onDealDefinitionSelectorReady = function (api) {
                    dealDefinitionSelectorAPI = api;
                    dealDefinitionSelectorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var carrierAccountLoadPromise = loadCarrierAccountSelectorPromise();
                    promises.push(carrierAccountLoadPromise);

                    var dealDefinitionPromise = loadDealDefinitionSelectorPromise();
                    if (dealDefinitionPromise != undefined)
                        promises.push(dealDefinitionPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                function loadCarrierAccountSelectorPromise() {
                    var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                    carrierAccountReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
                    });

                    return loadCarrierAccountPromiseDeferred.promise;
                }
                function loadDealDefinitionSelectorPromise() {
                    var loadDealPromiseDeferred = UtilsService.createPromiseDeferred();
                    VRUIUtilsService.callDirectiveLoad(dealDefinitionSelectorAPI, undefined, loadDealPromiseDeferred);

                    return loadDealPromiseDeferred.promise;
                }


                api.getSelectedIds = function () {
                    return dealDefinitionSelectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);