'use strict';
app.directive('vrGenericdataDatatransformationTariffrulestep', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new rateTypeRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Tariff/Templates/RateTypeRuleStepTemplate.html';
            }

        };

        function rateTypeRuleStepCtor(ctrl, $scope) {
            var ruleTypeName = "VR_TariffRule";
            var ruleTypeEntity;

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var initialRateDirectiveAPI;
            var initialRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var durationInSecondsDirectiveAPI;
            var durationInSecondsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveRateDirectiveAPI;
            var effectiveRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveDurationInSecondsDirectiveAPI;
            var effectiveDurationInSecondsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var totalAmountDirectiveAPI;
            var totalAmountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onInitialRateDirectiveReady = function (api) {
                    initialRateDirectiveAPI = api;
                    initialRateDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onDurationInSecondsDirectiveReady = function (api) {
                    durationInSecondsDirectiveAPI = api;
                    durationInSecondsDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onEffectiveRateDirectiveReady = function (api) {
                    effectiveRateDirectiveAPI = api;
                    effectiveRateDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onEffectiveDurationInSecondsDirectiveReady = function (api) {
                    effectiveDurationInSecondsDirectiveAPI = api;
                    effectiveDurationInSecondsDirectiveReadyPromiseDeferred.resolve();
                }
                $scope.onTotalAmountDirectiveReady = function (api) {
                    totalAmountDirectiveAPI = api;
                    totalAmountDirectiveReadyPromiseDeferred.resolve();
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
                            payloadRuleStep.effectiveTime = payload.stepDetails.EffectiveTime;
                            payloadRuleStep.ruleDefinitionId = payload.stepDetails.RuleDefinitionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payloadRuleStep, loadRuleStepCommonDirectivePromiseDeferred);
                    });

                    promises.push(loadRuleStepCommonDirectivePromiseDeferred.promise);

                    var loadInitialRateDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    initialRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadInitialRate;
                        if (payload != undefined) {
                            payloadInitialRate = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadInitialRate.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadInitialRate.selectedRecords = payload.stepDetails.InitialRate;
                        }
                        VRUIUtilsService.callDirectiveLoad(initialRateDirectiveAPI, payloadInitialRate, loadInitialRateDirectivePromiseDeferred);
                    });

                    promises.push(loadInitialRateDirectivePromiseDeferred.promise);

                    var loadDurationInSecondsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    durationInSecondsDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadDurationInSeconds;
                        if (payload != undefined) {
                            payloadDurationInSeconds = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadDurationInSeconds.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadDurationInSeconds.selectedRecords = payload.stepDetails.DurationInSeconds;
                        }
                        VRUIUtilsService.callDirectiveLoad(durationInSecondsDirectiveAPI, payloadDurationInSeconds, loadDurationInSecondsDirectivePromiseDeferred);
                    });

                    promises.push(loadDurationInSecondsDirectivePromiseDeferred.promise);



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


                    var loadEffectiveDurationInSecondsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    effectiveDurationInSecondsDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadEffectiveDurationInSeconds;
                        if (payload != undefined) {
                            payloadEffectiveDurationInSeconds = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadEffectiveDurationInSeconds.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadEffectiveDurationInSeconds.selectedRecords = payload.stepDetails.EffectiveDurationInSeconds;
                        }
                        VRUIUtilsService.callDirectiveLoad(rateTypeIdDirectiveAPI, payloadEffectiveDurationInSeconds, loadEffectiveDurationInSecondsDirectivePromiseDeferred);
                    });

                    promises.push(loadEffectiveDurationInSecondsDirectivePromiseDeferred.promise);

                    var loadTotalAmountDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    totalAmountDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadTotalAmount;
                        if (payload != undefined) {
                            payloadTotalAmount = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadTotalAmount.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadTotalAmount.selectedRecords = payload.stepDetails.TotalAmount;
                        }
                        VRUIUtilsService.callDirectiveLoad(totalAmountDirectiveAPI, payloadTotalAmount, loadTotalAmountDirectivePromiseDeferred);
                    });

                    promises.push(loadTotalAmountDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Pricing.TariffMappingStep, Vanrise.GenericData.Pricing",
                        InitialRate: initialRateDirectiveAPI != undefined ? initialRateDirectiveAPI.getData() : undefined,
                        DurationInSeconds: durationInSecondsDirectiveAPI != undefined ? durationInSecondsDirectiveAPI.getData() : undefined,
                        EffectiveRate: effectiveRateDirectiveAPI != undefined ? effectiveRateDirectiveAPI.getData() : undefined,
                        EffectiveDurationInSeconds: effectiveDurationInSecondsDirectiveAPI != undefined ? effectiveDurationInSecondsDirectiveAPI.getData() : undefined,
                        TotalAmount: totalAmountDirectiveAPI != undefined ? totalAmountDirectiveAPI.getData() : undefined,
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