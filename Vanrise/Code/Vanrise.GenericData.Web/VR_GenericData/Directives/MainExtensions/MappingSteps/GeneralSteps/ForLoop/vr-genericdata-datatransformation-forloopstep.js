'use strict';
app.directive('vrGenericdataDatatransformationForloopstep', ['UtilsService','VR_GenericData_GenericRuleTypeConfigAPIService','VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new ForLoopStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/ForLoop/Templates/ForLoopStepTemplate.html';
            }

        };

        function ForLoopStepCtor(ctrl, $scope) {

            function initializeController() {               
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.context != undefined)
                    {
                        ctrl.arrayRecords = payload.context.getArrayRecordNames();
                    }
                    if (payload.stepDetails != undefined) {
                        ctrl.iterationVariableName = payload.stepDetails.IterationVariableName;
                        ctrl.selectedArray = UtilsService.getItemByVal(ctrl.arrayRecords, payload.stepDetails.ArrayVariableName, "Name");
                    }
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.ForLoopStep, Vanrise.GenericData.Transformation.MainExtensions",
                        IterationVariableName: ctrl.iterationVariableName,
                        ArrayVariableName: ctrl.selectedArray != undefined? ctrl.selectedArray.Name:undefined
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