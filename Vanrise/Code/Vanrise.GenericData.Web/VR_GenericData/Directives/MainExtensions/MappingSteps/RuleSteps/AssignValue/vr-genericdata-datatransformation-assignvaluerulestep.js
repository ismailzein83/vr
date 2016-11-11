'use strict';
app.directive('vrGenericdataDatatransformationAssignvaluerulestep', ['UtilsService','VR_GenericData_GenericRuleTypeConfigAPIService','VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new assignValueRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/AssignValue/Templates/AssignValueRuleStepTemplate.html';
            }

        };

        function assignValueRuleStepCtor(ctrl, $scope) {
            var ruleTypeName = "VR_MappingRule";
            var ruleTypeEntity;

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var targetDirectiveAPI;
            var targetDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTargetDirectiveReady = function (api) {
                    targetDirectiveAPI = api;
                    targetDirectiveReadyPromiseDeferred.resolve();
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
                            payloadRuleStep.effectiveTime = payload.stepDetails.EffectiveTime;
                            payloadRuleStep.isEffectiveInFuture = payload.stepDetails.IsEffectiveInFuture;
                            payloadRuleStep.ruleDefinitionId = payload.stepDetails.RuleDefinitionId;
                            payloadRuleStep.ruleId = payload.stepDetails.RuleId;
                        }
                        VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payloadRuleStep, loadRuleStepCommonDirectivePromiseDeferred);
                    });

                    promises.push(loadRuleStepCommonDirectivePromiseDeferred.promise);

                    var loadTargetDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    targetDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadtarget;
                        if (payload != undefined) {
                            payloadtarget = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadtarget.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadtarget.selectedRecords = payload.stepDetails.Target;
                        }
                        VRUIUtilsService.callDirectiveLoad(targetDirectiveAPI, payloadtarget, loadTargetDirectivePromiseDeferred);
                    });

                    promises.push(loadTargetDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };
               
                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignValueRuleStep, Vanrise.GenericData.Transformation.MainExtensions",
                        Target: targetDirectiveAPI != undefined ? targetDirectiveAPI.getData() : undefined
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