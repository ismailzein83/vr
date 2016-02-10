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

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Pricing.ExtraChargeMappingStep, Vanrise.GenericData.Pricing",
                        InitialRate: initialRateDirectiveAPI != undefined ? initialRateDirectiveAPI.getData() : undefined,
                        EffectiveRate: effectiveRateDirectiveAPI != undefined ? effectiveRateDirectiveAPI.getData() : undefined,
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