'use strict';
app.directive('vrGenericdataDatatransformationAssignfieldstep', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new AssignFieldStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/Templates/AssignFieldStepTemplate.html';
            }

        };

        function AssignFieldStepCtor(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) { }

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions",
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);