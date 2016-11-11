"use strict";

app.directive("vrGenericdataSummarytransformationTimeintervalSelector", ['VR_GenericData_IntervalTypeEnum', 'UtilsService',
    function (VR_GenericData_IntervalTypeEnum, UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return getDirectiveTemplateUrl();
            }
        };

        function getDirectiveTemplateUrl() {
            return "/Client/Modules/VR_GenericData/Directives/MainExtensions/SummaryBatchTimeIntervalRange/Templates/TimeIntervalSelector.html";
        }

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;
            $scope.intervalTypes = UtilsService.getArrayEnum(VR_GenericData_IntervalTypeEnum);

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.SummaryBatchTimeIntervalRange, Vanrise.GenericData.MainExtensions",
                        IntervalOffset: $scope.intervalOffset,
                        IntervalType: $scope.selectedIntervalType.value
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.intervalOffset = payload.Settings.IntervalOffset;
                        $scope.selectedIntervalType = UtilsService.getItemByVal($scope.intervalTypes, payload.Settings.IntervalType, "value");
                    }
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
