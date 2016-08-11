'use strict';
app.directive('vrGenericdataDatatransformationRulestepcommonPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new commonRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Common/Templates/RuleStepCommonPreviewTemplate.html';
            }

        };

        function commonRuleStepCtor(ctrl, $scope) {
            var stepObj = {};

            function initializeController() {
                ctrl.ruleFieldsMappings = [];
                ctrl.ruleObjectsMappings = [];


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.ruleFieldsMappings.length = 0;
                    ctrl.ruleObjectsMappings.length = 0;

                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;
                            ctrl.ruleFieldsMappings = payload.stepDetails.RuleFieldsMappings;
                            ctrl.ruleObjectsMappings = payload.stepDetails.RuleObjectsMappings;
                            ctrl.effectiveTime = payload.stepDetails.EffectiveTime;
                            ctrl.isEffectiveInFuture = payload.stepDetails.IsEffectiveInFuture;
                            ctrl.ruleId = payload.stepDetails.RuleId;
                        }

                    }

                }

                api.applyChanges = function (changes) {
                    stepObj.stepDetails = changes;
                    ctrl.ruleFieldsMappings = changes.RuleFieldsMappings;
                    ctrl.ruleObjectsMappings = changes.RuleObjectsMappings;
                    ctrl.effectiveTime = changes.EffectiveTime;
                    ctrl.isEffectiveInFuture = changes.IsEffectiveInFuture;
                    ctrl.ruleId = changes.RuleId;
                }

                api.checkValidation = function () {
                    return checkValidation();
                }

                api.getData = function () {
                    return stepObj.stepDetails;
                }


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.ruleObjectsMappings != undefined)
                {
                    if (!checkRuleObjectsFilled(ctrl.ruleObjectsMappings))
                    {
                        if (ctrl.ruleFieldsMappings != undefined) {
                            for (var i = 0 ; i < ctrl.ruleFieldsMappings.length; i++) {
                                if (ctrl.ruleFieldsMappings[i].Value == undefined)
                                    return "All fields should be mapped.";
                            }
                        } else {
                            return "All fields should be mapped.";
                        }
                    }
                }

                return null;
            }
            function checkRuleObjectsFilled(ruleObjectsMappings)
            {
                for (var i = 0 ; i < ruleObjectsMappings.length; i++) {
                    if (ruleObjectsMappings[i].Value != undefined)
                        return true;
                }
                return false;
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);