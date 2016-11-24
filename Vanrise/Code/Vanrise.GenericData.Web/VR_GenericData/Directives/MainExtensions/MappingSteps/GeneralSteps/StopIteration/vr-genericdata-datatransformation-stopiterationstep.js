'use strict';

app.directive('vrGenericdataDatatransformationStopiterationstep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new StopIterationStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/StopIteration/Templates/StopIterationStepTemplate.html';
            }

        };

        function StopIterationStepCtor(ctrl, $scope) {
            var stepPayload;

            function initializeController() {

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    stepPayload = payload;

                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.StopIterationStep, Vanrise.GenericData.Transformation.MainExtensions"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);