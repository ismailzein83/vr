'use strict';
app.directive('vrGenericdataDatatransformationRatetyperulestep', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/RateType/Templates/RateTypeRuleStepTemplate.html';
            }

        };

        function rateTypeRuleStepCtor(ctrl, $scope) {
            var ruleTypeName = "VR_RateTypeRule";
            var ruleTypeEntity;

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var normalRateDirectiveAPI;
            var normalRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var ratesByRateTypeDirectiveAPI;
            var ratesByRateTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveRateDirectiveAPI;
            var effectiveRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var rateTypeIdDirectiveAPI;
            var rateTypeIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onNormalRateDirectiveReady = function (api) {
                    normalRateDirectiveAPI = api;
                    normalRateDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onRatesByRateTypeDirectiveReady = function (api) {
                    ratesByRateTypeDirectiveAPI = api;
                    ratesByRateTypeDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onEffectiveRateDirectiveReady = function (api) {
                    effectiveRateDirectiveAPI = api;
                    effectiveRateDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onRateTypeIdDirectiveReady = function (api) {
                    rateTypeIdDirectiveAPI = api;
                    rateTypeIdDirectiveReadyPromiseDeferred.resolve();
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
                            payloadRuleStep.ruleDefinitionId = payload.stepDetails.RuleDefinitionId;
                            payloadRuleStep.ruleId = payload.stepDetails.RuleId;
                        }
                        VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payloadRuleStep, loadRuleStepCommonDirectivePromiseDeferred);
                    });

                    promises.push(loadRuleStepCommonDirectivePromiseDeferred.promise);

                    var loadNormalRateDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    normalRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadNormalRate;
                        if (payload != undefined) {
                            payloadNormalRate = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadNormalRate.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadNormalRate.selectedRecords = payload.stepDetails.NormalRate;
                        }
                        VRUIUtilsService.callDirectiveLoad(normalRateDirectiveAPI, payloadNormalRate, loadNormalRateDirectivePromiseDeferred);
                    });

                    promises.push(loadNormalRateDirectivePromiseDeferred.promise);

                    var loadRatesByRateTypeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    ratesByRateTypeDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRatesByRateType;
                        if (payload != undefined) {
                            payloadRatesByRateType = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadRatesByRateType.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadRatesByRateType.selectedRecords = payload.stepDetails.RatesByRateType;
                        }
                        VRUIUtilsService.callDirectiveLoad(ratesByRateTypeDirectiveAPI, payloadRatesByRateType, loadRatesByRateTypeDirectivePromiseDeferred);
                    });

                    promises.push(loadRatesByRateTypeDirectivePromiseDeferred.promise);



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


                    var loadRateTypeIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    rateTypeIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRatesByRateType;
                        if (payload != undefined) {
                            payloadRatesByRateType = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadRatesByRateType.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadRatesByRateType.selectedRecords = payload.stepDetails.RateTypeId;
                        }
                        VRUIUtilsService.callDirectiveLoad(rateTypeIdDirectiveAPI, payloadRatesByRateType, loadRateTypeIdDirectivePromiseDeferred);
                    });

                    promises.push(loadRateTypeIdDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Pricing.RateTypeMappingStep, Vanrise.GenericData.Pricing",
                        NormalRate: normalRateDirectiveAPI != undefined ? normalRateDirectiveAPI.getData() : undefined,
                        RatesByRateType: ratesByRateTypeDirectiveAPI != undefined ? ratesByRateTypeDirectiveAPI.getData() : undefined,
                        EffectiveRate: effectiveRateDirectiveAPI != undefined ? effectiveRateDirectiveAPI.getData() : undefined,
                        RateTypeId: rateTypeIdDirectiveAPI != undefined ? rateTypeIdDirectiveAPI.getData() : undefined,
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