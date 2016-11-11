'use strict';
app.directive('vrGenericdataDatatransformationExecutetransformationstepPreview', ['UtilsService', 'VRUIUtilsService',
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/ExecuteTransformation/Templates/ExecuteTransformationStepPreviewTemplate.html';
            }

        };

        function commonRuleStepCtor(ctrl, $scope) {
            var stepObj = {};

            function initializeController() {
                ctrl.recordsMapping = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.recordsMapping.length = 0;
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;
                            if (payload.stepDetails.RecordsMapping != undefined && payload.stepDetails.RecordsMapping.length > 0) {
                                ctrl.recordsMapping = payload.stepDetails.RecordsMapping;
                            }

                        }

                    }

                };

                api.applyChanges = function (changes) {
                    stepObj.stepDetails = changes;
                    ctrl.recordsMapping = changes.RecordsMapping;
                };

                api.checkValidation = function () {
                    return checkValidation();
                };

                api.getData = function () {
                    return stepObj.stepDetails;
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.recordsMapping != undefined) {
                    for (var i = 0 ; i < ctrl.recordsMapping.length; i++) {
                        if (ctrl.recordsMapping[i].Value == undefined)
                            return "All fields should be mapped.";
                    }
                } else {
                    return "All fields should be mapped.";
                }

                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);