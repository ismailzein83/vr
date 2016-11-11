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

            var rateTypesDirectiveAPI;
            var rateTypesDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var rateTypeIdDirectiveAPI;
            var rateTypeIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onRateTypesDirectiveReady = function (api) {
                    rateTypesDirectiveAPI = api;
                    rateTypesDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onRateTypeIdDirectiveReady = function (api) {
                    rateTypeIdDirectiveAPI = api;
                    rateTypeIdDirectiveReadyPromiseDeferred.resolve();
                };

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

                    var loadRateTypesDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    rateTypesDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRateTypes;
                        if (payload != undefined) {
                            payloadRateTypes = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadRateTypes.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadRateTypes.selectedRecords = payload.stepDetails.RateTypes;
                        }
                        VRUIUtilsService.callDirectiveLoad(rateTypesDirectiveAPI, payloadRateTypes, loadRateTypesDirectivePromiseDeferred);
                    });

                    promises.push(loadRateTypesDirectivePromiseDeferred.promise);

                    var loadRateTypeIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    rateTypeIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRateTypeId;
                        if (payload != undefined) {
                            payloadRateTypeId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadRateTypeId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadRateTypeId.selectedRecords = payload.stepDetails.RateTypeId;
                        }
                        VRUIUtilsService.callDirectiveLoad(rateTypeIdDirectiveAPI, payloadRateTypeId, loadRateTypeIdDirectivePromiseDeferred);
                    });

                    promises.push(loadRateTypeIdDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Pricing.RateTypeMappingStep, Vanrise.GenericData.Pricing",
                        RateTypes: rateTypesDirectiveAPI != undefined ? rateTypesDirectiveAPI.getData() : undefined,
                        RateTypeId: rateTypeIdDirectiveAPI != undefined ? rateTypeIdDirectiveAPI.getData() : undefined,
                    };
                    ruleStepCommonDirectiveAPI.setData(obj);
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