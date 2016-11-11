'use strict';
app.directive('vrGenericdataDatatransformationNormalizationrulestep', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new normalizationruleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Normalization/Templates/NormalizationRuleStepTemplate.html';
            }

        };

        function normalizationruleStepCtor(ctrl, $scope) {
            var ruleTypeName = "VR_NormalizationRule";
            var ruleTypeEntity;

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var valueDirectiveAPI;
            var valueDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var normalizationValueDirectiveAPI;
            var normalizationValueDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onValueDirectiveReady = function (api) {
                    valueDirectiveAPI = api;
                    valueDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onNormalizationValueDirectiveReady = function (api) {
                    normalizationValueDirectiveAPI = api;
                    normalizationValueDirectiveReadyPromiseDeferred.resolve();
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
                            payloadRuleStep.ruleDefinitionId = payload.stepDetails.RuleDefinitionId;
                            payloadRuleStep.ruleId = payload.stepDetails.RuleId;
                        }
                        VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payloadRuleStep, loadRuleStepCommonDirectivePromiseDeferred);
                    });

                    promises.push(loadRuleStepCommonDirectivePromiseDeferred.promise);

                    var loadValueDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    valueDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadValue;
                        if (payload != undefined) {
                            payloadValue = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadValue.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadValue.selectedRecords = payload.stepDetails.Value;
                        }
                        VRUIUtilsService.callDirectiveLoad(valueDirectiveAPI, payloadValue, loadValueDirectivePromiseDeferred);
                    });

                    promises.push(loadValueDirectivePromiseDeferred.promise);


                    var loadNormalizationValueDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    normalizationValueDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadValue;
                        if (payload != undefined) {
                            payloadValue = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadValue.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadValue.selectedRecords = payload.stepDetails.NormalizedValue;
                        }
                        VRUIUtilsService.callDirectiveLoad(normalizationValueDirectiveAPI, payloadValue, loadNormalizationValueDirectivePromiseDeferred);
                    });

                    promises.push(loadNormalizationValueDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Normalization.NormalizationRuleStep, Vanrise.GenericData.Normalization",
                        Value: valueDirectiveAPI != undefined ? valueDirectiveAPI.getData() : undefined,
                        NormalizedValue: normalizationValueDirectiveAPI != undefined ? normalizationValueDirectiveAPI.getData() : undefined
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