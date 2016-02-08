'use strict';
app.directive('vrGenericdataDatatransformationAssignvaluerulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/AssignValue/Templates/AssignValueRuleStepPreviewTemplate.html';
            }

        };

        function assignValueRuleStepCtor(ctrl, $scope) {
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
                            stepObj.configId = payload.stepDetails.ConfigId;
                            if (payload.stepDetails.RuleFieldsMappings != undefined && payload.stepDetails.RuleFieldsMappings.length>0)
                             ctrl.ruleFieldsMappings = payload.stepDetails.RuleFieldsMappings;
                            ctrl.target = payload.stepDetails.Target;
                        }

                    }

                }

                api.applyChanges = function (changes) {
                    stepObj.stepDetails = changes;
                    ctrl.ruleFieldsMappings = changes.RuleFieldsMappings;
                    ctrl.target = changes.Target;
                }

                api.checkValidation = function () {
                    return checkValidation();
                }

                api.getData = function () {
                    stepObj.stepDetails.ConfigId = stepObj.configId;
                    return stepObj.stepDetails
                }


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.ruleFieldsMappings != undefined)
                {
                    for (var i = 0 ; i < ctrl.ruleFieldsMappings.length; i++) {
                        if(ctrl.ruleFieldsMappings[i].Value == undefined)
                            return "All fields should be mapped.";
                    }
                } else
                {
                    return "All fields should be mapped.";
                }
                
                if (ctrl.target == undefined) {
                    return "Missing target mapping.";
                }
                if (stepObj.stepDetails.EffectiveTime == undefined)
                    return "Missing effective time mapping.";

                return null;
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);