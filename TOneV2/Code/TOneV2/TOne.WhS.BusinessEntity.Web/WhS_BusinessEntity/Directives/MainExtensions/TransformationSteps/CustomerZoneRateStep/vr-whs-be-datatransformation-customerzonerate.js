'use strict';
app.directive('vrWhsBeDatatransformationCustomerzonerate', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new CustomerZoneRateMatchStepCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/TransformationSteps/CustomerZoneRateStep/Templates/CustomerZoneRateStepTemplate.html';
            }

        };

        function CustomerZoneRateMatchStepCtor(ctrl, $scope) {

            var customerIdDirectiveAPI;
            var customerIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneIdDirectiveAPI;
            var saleZoneIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveOnDirectiveAPI;
            var effectiveOnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerZoneRateDirectiveAPI;
            var customerZoneRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onCustomerIdDirectiveReady = function (api) {
                    customerIdDirectiveAPI = api;
                    customerIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onSaleZoneIdDirectiveReady = function (api) {
                    saleZoneIdDirectiveAPI = api;
                    saleZoneIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onEffectiveOnDirectiveReady = function (api) {
                    effectiveOnDirectiveAPI = api;
                    effectiveOnDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onCustomerZoneRateDirectiveReady = function (api) {
                    customerZoneRateDirectiveAPI = api;
                    customerZoneRateDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadCustomerIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    customerIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadCustomerId;
                        if (payload != undefined) {
                            payloadCustomerId = {};
                            payloadCustomerId.context = payload.context;
                            if (payload.stepDetails != undefined)
                                payloadCustomerId.selectedRecords = payload.stepDetails.CustomerId;
                        }
                        VRUIUtilsService.callDirectiveLoad(customerIdDirectiveAPI, payloadCustomerId, loadCustomerIdDirectivePromiseDeferred);
                    });

                    promises.push(loadCustomerIdDirectivePromiseDeferred.promise);

                    var loadSaleZoneIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    saleZoneIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSaleZoneId;
                        if (payload != undefined) {
                            payloadSaleZoneId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadSaleZoneId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadSaleZoneId.selectedRecords = payload.stepDetails.SaleZoneId;
                        }
                        VRUIUtilsService.callDirectiveLoad(saleZoneIdDirectiveAPI, payloadSaleZoneId, loadSaleZoneIdDirectivePromiseDeferred);
                    });

                    promises.push(loadSaleZoneIdDirectivePromiseDeferred.promise);

                    var loadEffectiveOnDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    effectiveOnDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadEffectiveOn;
                        if (payload != undefined) {
                            payloadEffectiveOn = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadEffectiveOn.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadEffectiveOn.selectedRecords = payload.stepDetails.EffectiveOn;
                        }
                        VRUIUtilsService.callDirectiveLoad(effectiveOnDirectiveAPI, payloadEffectiveOn, loadEffectiveOnDirectivePromiseDeferred);
                    });

                    promises.push(loadEffectiveOnDirectivePromiseDeferred.promise);


                    var loadCustomerZoneRateDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    customerZoneRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadCustomerZoneRate;
                        if (payload != undefined) {
                            payloadCustomerZoneRate = {};
                            payloadCustomerZoneRate.context = payload.context;
                            if (payload.stepDetails != undefined)
                                payloadCustomerZoneRate.selectedRecords = payload.stepDetails.CustomerZoneRate;
                        }
                        VRUIUtilsService.callDirectiveLoad(customerZoneRateDirectiveAPI, payloadCustomerZoneRate, loadCustomerZoneRateDirectivePromiseDeferred);
                    });

                    promises.push(loadCustomerZoneRateDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.TransformationSteps.CustomerZoneRateStep, TOne.WhS.BusinessEntity.MainExtensions",
                        CustomerId: customerIdDirectiveAPI != undefined ? customerIdDirectiveAPI.getData() : undefined,
                        SaleZoneId: saleZoneIdDirectiveAPI != undefined ? saleZoneIdDirectiveAPI.getData() : undefined,
                        EffectiveOn: effectiveOnDirectiveAPI != undefined ? effectiveOnDirectiveAPI.getData() : undefined,
                        CustomerZoneRate: customerZoneRateDirectiveAPI != undefined ? customerZoneRateDirectiveAPI.getData() : undefined
                    }
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);