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
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.ruleFieldsMappings.length = 0;
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;
                            if (payload.stepDetails.RuleFieldsMappings != undefined && payload.stepDetails.RuleFieldsMappings.length > 0)
                            {
                                ctrl.ruleFieldsMappings = payload.stepDetails.RuleFieldsMappings;
                                ctrl.effectiveTime = payload.stepDetails.EffectiveTime
                            }
                              
                        }

                    }

                }

                api.applyChanges = function (changes) {
                    stepObj.stepDetails = changes;
                    ctrl.ruleFieldsMappings = changes.RuleFieldsMappings;
                    ctrl.effectiveTime = changes.EffectiveTime;
                }

                api.checkValidation = function () {
                    return checkValidation();
                }

                api.getData = function () {
                    return stepObj.stepDetails
                }


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.ruleFieldsMappings != undefined) {
                    for (var i = 0 ; i < ctrl.ruleFieldsMappings.length; i++) {
                        if (ctrl.ruleFieldsMappings[i].Value == undefined)
                            return "All fields should be mapped.";
                    }
                } else {
                    return "All fields should be mapped.";
                }
                if (ctrl.effectiveTime == undefined)
                    return "Missing effective time mapping.";

                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);