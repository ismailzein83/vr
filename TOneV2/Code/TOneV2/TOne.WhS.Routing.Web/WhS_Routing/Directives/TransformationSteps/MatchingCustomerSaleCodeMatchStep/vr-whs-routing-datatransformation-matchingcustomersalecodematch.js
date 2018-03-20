'use strict';
app.directive('vrWhsRoutingDatatransformationMatchingcustomersalecodematch', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SaleCodeMatchStepCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/TransformationSteps/MatchingCustomerSaleCodeMatchStep/Templates/MatchingCustomerSaleCodeMatchStepTemplate.html';
            }

        };

        function SaleCodeMatchStepCtor(ctrl, $scope) {
            var numberDirectiveAPI;
            var numberRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerIdDirectiveAPI;
            var customerIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerANumberIdDirectiveAPI;
            var customerANumberIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveOnDirectiveAPI;
            var effectiveOnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var matchingCustomerIdDirectiveAPI;
            var matchingCustomerIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerNumberPlanDirectiveAPI;
            var customerNumberPlanDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleCodeDirectiveAPI;
            var saleCodeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneIdDirectiveAPI;
            var saleZoneIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var masterSaleCodeDirectiveAPI;
            var masterSaleCodeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var masterSaleZoneIdDirectiveAPI;
            var masterSaleZoneIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onNumberDirectiveReady = function (api) {
                    numberDirectiveAPI = api;
                    numberRateDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onCustomerIdDirectiveReady = function (api) {
                    customerIdDirectiveAPI = api;
                    customerIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onCustomerANumberIdDirectiveReady = function (api) {
                    customerANumberIdDirectiveAPI = api;
                    customerANumberIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onEffectiveOnDirectiveReady = function (api) {
                    effectiveOnDirectiveAPI = api;
                    effectiveOnDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onMatchingCustomerIdDirectiveReady = function (api) {
                    matchingCustomerIdDirectiveAPI = api;
                    matchingCustomerIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onCustomerNumberPlanDirectiveReady = function (api) {
                    customerNumberPlanDirectiveAPI = api;
                    customerNumberPlanDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onSaleCodeDirectiveReady = function (api) {
                    saleCodeDirectiveAPI = api;
                    saleCodeDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onSaleZoneIdDirectiveReady = function (api) {
                    saleZoneIdDirectiveAPI = api;
                    saleZoneIdDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onMasterSaleCodeDirectiveReady = function (api) {
                    masterSaleCodeDirectiveAPI = api;
                    masterSaleCodeDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onMasterSaleZoneIdDirectiveReady = function (api) {
                    masterSaleZoneIdDirectiveAPI = api;
                    masterSaleZoneIdDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadNumberDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    numberRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadNumber;
                        if (payload != undefined) {
                            payloadNumber = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadNumber.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadNumber.selectedRecords = payload.stepDetails.Number;
                        }
                        VRUIUtilsService.callDirectiveLoad(numberDirectiveAPI, payloadNumber, loadNumberDirectivePromiseDeferred);
                    });

                    promises.push(loadNumberDirectivePromiseDeferred.promise);


                    var loadCustomerIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    customerIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadCustomerId;
                        if (payload != undefined) {
                            payloadCustomerId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadCustomerId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadCustomerId.selectedRecords = payload.stepDetails.CustomerId;
                        }
                        VRUIUtilsService.callDirectiveLoad(customerIdDirectiveAPI, payloadCustomerId, loadCustomerIdDirectivePromiseDeferred);
                    });

                    promises.push(loadCustomerIdDirectivePromiseDeferred.promise);

                    var loadCustomerANumberIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    customerANumberIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadCustomerANumberId;
                        if (payload != undefined) {
                            payloadCustomerANumberId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadCustomerANumberId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadCustomerANumberId.selectedRecords = payload.stepDetails.CustomerANumberId;
                        }
                        VRUIUtilsService.callDirectiveLoad(customerANumberIdDirectiveAPI, payloadCustomerANumberId, loadCustomerANumberIdDirectivePromiseDeferred);
                    });

                    promises.push(loadCustomerANumberIdDirectivePromiseDeferred.promise);

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



                    var loadMatchingCustomerIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    matchingCustomerIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadMatchingCustomerId;
                        if (payload != undefined) {
                            payloadMatchingCustomerId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadMatchingCustomerId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadMatchingCustomerId.selectedRecords = payload.stepDetails.MatchingCustomerId;
                        }
                        VRUIUtilsService.callDirectiveLoad(matchingCustomerIdDirectiveAPI, payloadMatchingCustomerId, loadMatchingCustomerIdDirectivePromiseDeferred);
                    });

                    promises.push(loadMatchingCustomerIdDirectivePromiseDeferred.promise);

                    var loadCustomerNumberPlanDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    customerNumberPlanDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadCustomerNumberPlan;
                        if (payload != undefined) {
                            payloadCustomerNumberPlan = {};
                            if (payload != undefined) {
                                payloadCustomerNumberPlan.context = payload.context;
                                if (payload.stepDetails != undefined)
                                    payloadCustomerNumberPlan.selectedRecords = payload.stepDetails.CustomerSellingNumberPlanId;
                            }
                        }
                        VRUIUtilsService.callDirectiveLoad(customerNumberPlanDirectiveAPI, payloadCustomerNumberPlan, loadCustomerNumberPlanDirectivePromiseDeferred);
                    });

                    promises.push(loadCustomerNumberPlanDirectivePromiseDeferred.promise);

                    var loadSaleCodeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    saleCodeDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSaleCode;
                        if (payload != undefined) {
                            payloadSaleCode = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadSaleCode.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadSaleCode.selectedRecords = payload.stepDetails.SaleCode;
                        }
                        VRUIUtilsService.callDirectiveLoad(saleCodeDirectiveAPI, payloadSaleCode, loadSaleCodeDirectivePromiseDeferred);
                    });

                    promises.push(loadSaleCodeDirectivePromiseDeferred.promise);


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


                    var loadMasterSaleCodeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    masterSaleCodeDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadMasterSaleCode;
                        if (payload != undefined) {
                            payloadMasterSaleCode = {};
                            if (payload != undefined) {
                                payloadMasterSaleCode.context = payload.context;
                                if (payload.stepDetails != undefined)
                                    payloadMasterSaleCode.selectedRecords = payload.stepDetails.MasterSaleCode;
                            }
                        }
                        VRUIUtilsService.callDirectiveLoad(masterSaleCodeDirectiveAPI, payloadMasterSaleCode, loadMasterSaleCodeDirectivePromiseDeferred);
                    });

                    promises.push(loadMasterSaleCodeDirectivePromiseDeferred.promise);


                    var loadMasterSaleZoneIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    masterSaleZoneIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadMasterSaleZoneId;
                        if (payload != undefined) {
                            payloadMasterSaleZoneId = {};
                            if (payload != undefined) {
                                payloadMasterSaleZoneId.context = payload.context;
                                if (payload.stepDetails != undefined)
                                    payloadMasterSaleZoneId.selectedRecords = payload.stepDetails.MasterSaleZoneId;
                            }
                        }
                        VRUIUtilsService.callDirectiveLoad(masterSaleZoneIdDirectiveAPI, payloadMasterSaleZoneId, loadMasterSaleZoneIdDirectivePromiseDeferred);
                    });

                    promises.push(loadMasterSaleCodeDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.Routing.Business.TransformationSteps.MatchingCustomerSaleCodeMatchStep, TOne.WhS.Routing.Business",
                        Number: numberDirectiveAPI != undefined ? numberDirectiveAPI.getData() : undefined,
                        CustomerId: customerIdDirectiveAPI != undefined ? customerIdDirectiveAPI.getData() : undefined,
                        CustomerANumberId: customerANumberIdDirectiveAPI != undefined ? customerANumberIdDirectiveAPI.getData() : undefined,
                        EffectiveOn: effectiveOnDirectiveAPI != undefined ? effectiveOnDirectiveAPI.getData() : undefined,
                        MatchingCustomerId: matchingCustomerIdDirectiveAPI != undefined ? matchingCustomerIdDirectiveAPI.getData() : undefined,
                        CustomerSellingNumberPlanId: customerNumberPlanDirectiveAPI != undefined ? customerNumberPlanDirectiveAPI.getData() : undefined,
                        SaleCode: saleCodeDirectiveAPI != undefined ? saleCodeDirectiveAPI.getData() : undefined,
                        SaleZoneId: saleZoneIdDirectiveAPI != undefined ? saleZoneIdDirectiveAPI.getData() : undefined,
                        MasterSaleCode: masterSaleCodeDirectiveAPI != undefined ? masterSaleCodeDirectiveAPI.getData() : undefined,
                        MasterSaleZoneId: masterSaleZoneIdDirectiveAPI != undefined ? masterSaleZoneIdDirectiveAPI.getData() : undefined
                    };
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