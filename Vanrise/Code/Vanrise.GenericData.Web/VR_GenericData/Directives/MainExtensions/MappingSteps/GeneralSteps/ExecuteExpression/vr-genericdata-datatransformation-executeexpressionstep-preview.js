﻿'use strict';
app.directive('vrGenericdataDatatransformationExecuteexpressionstepPreview', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new ExecuteExpressionStepCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/ExecuteExpression/Templates/ExecuteExpressionStepPreviewTemplate.html';
            }

        };

        function ExecuteExpressionStepCtor(ctrl, $scope) {
            var stepObj = {};

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;
                            ctrl.expression = payload.stepDetails.Expression;
                        }
                        checkValidation();
                    }

                };

                api.applyChanges = function (changes) {
                    ctrl.expression = changes.Expression;
                    stepObj.stepDetails = changes;
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
                if (ctrl.expression == undefined) {
                    return "Missing Expression value.";
                }
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);