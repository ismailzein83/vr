'use strict';
app.directive('vrGenericdataDatatransformationExtrachargerulestep', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new ExtraChargeRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/ExtraCharge/Templates/ExtraChargeRuleStepTemplate.html';
            }

        };

        function ExtraChargeRuleStepCtor(ctrl, $scope) {
            var ruleTypeName = "VR_ExtraChargeRule";
            var ruleTypeEntity;

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var initialRateDirectiveAPI;
            var initialRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveRateDirectiveAPI;
            var effectiveRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var extraChargeRateDirectiveAPI;
            var extraChargeRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyIdDirectiveAPI;
            var currencyIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onInitialRateDirectiveReady = function (api) {
                    initialRateDirectiveAPI = api;
                    initialRateDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onEffectiveRateDirectiveReady = function (api) {
                    effectiveRateDirectiveAPI = api;
                    effectiveRateDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onExtraChargeRateDirectiveReady = function (api) {
                    extraChargeRateDirectiveAPI = api;
                    extraChargeRateDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onCurrencyIdDirectiveReady = function (api) {
                    currencyIdDirectiveAPI = api;
                    currencyIdDirectiveReadyPromiseDeferred.resolve();
                }
                
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var loadRuleStepCommonDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    ruleStepCommonDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRuleStep = { ruleTypeName: ruleTypeName };
                        if (payload != undefined && payload.context != undefined)
                            payloadRuleStep.context = payload.context;
                        if (payload != undefined && payload.stepDetails) {
                            payloadRuleStep.ruleFieldsMappings = payload.stepDetails.RuleFieldsMappings;
                            payloadRuleStep.ruleObjectsMappings = payload.stepDetails.RuleObjectsMappings;
                            payloadRuleStep.effectiveTime = payload.stepDetails.EffectiveTime;
                            payloadRuleStep.isEffectiveInFuture = payload.stepDetails.IsEffectiveInFuture;
                            payloadRuleStep.ruleDefinitionId = payload.stepDetails.RuleDefinitionId;
                            payloadRuleStep.ruleId = payload.stepDetails.RuleId;
                        }
                        VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payloadRuleStep, loadRuleStepCommonDirectivePromiseDeferred);
                    });

                    promises.push(loadRuleStepCommonDirectivePromiseDeferred.promise);

                    var loadInitialRateDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    initialRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadNormalRate;
                        if (payload != undefined) {
                            payloadNormalRate = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadNormalRate.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadNormalRate.selectedRecords = payload.stepDetails.InitialRate;
                        }
                        VRUIUtilsService.callDirectiveLoad(initialRateDirectiveAPI, payloadNormalRate, loadInitialRateDirectivePromiseDeferred);
                    });

                    promises.push(loadInitialRateDirectivePromiseDeferred.promise);


                    var loadEffectiveRateDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    effectiveRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadEffectiveRate;
                        if (payload != undefined) {
                            payloadEffectiveRate = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadEffectiveRate.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadEffectiveRate.selectedRecords = payload.stepDetails.EffectiveRate;
                        }
                        VRUIUtilsService.callDirectiveLoad(effectiveRateDirectiveAPI, payloadEffectiveRate, loadEffectiveRateDirectivePromiseDeferred);
                    });

                    promises.push(loadEffectiveRateDirectivePromiseDeferred.promise);

                    var loadExtraChargeRateDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    extraChargeRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadExtraChargeRate;
                        if (payload != undefined) {
                            payloadExtraChargeRate = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadExtraChargeRate.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadExtraChargeRate.selectedRecords = payload.stepDetails.ExtraChargeRate;
                        }
                        VRUIUtilsService.callDirectiveLoad(extraChargeRateDirectiveAPI, payloadExtraChargeRate, loadExtraChargeRateDirectivePromiseDeferred);
                    });

                    promises.push(loadExtraChargeRateDirectivePromiseDeferred.promise);


                    var loadCurrencyIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    currencyIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadCurrencyId;
                        if (payload != undefined) {
                            payloadCurrencyId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadCurrencyId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadCurrencyId.selectedRecords = payload.stepDetails.CurrencyId;
                        }
                        VRUIUtilsService.callDirectiveLoad(currencyIdDirectiveAPI, payloadCurrencyId, loadCurrencyIdDirectivePromiseDeferred);
                    });

                    promises.push(loadCurrencyIdDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Pricing.ExtraChargeMappingStep, Vanrise.GenericData.Pricing",
                        InitialRate: initialRateDirectiveAPI != undefined ? initialRateDirectiveAPI.getData() : undefined,
                        EffectiveRate: effectiveRateDirectiveAPI != undefined ? effectiveRateDirectiveAPI.getData() : undefined,
                        ExtraChargeRate: extraChargeRateDirectiveAPI != undefined ? extraChargeRateDirectiveAPI.getData() : undefined,
                        CurrencyId: currencyIdDirectiveAPI != undefined ? currencyIdDirectiveAPI.getData() : undefined
                    }
                    ruleStepCommonDirectiveAPI.setData(obj);
                    return obj;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);