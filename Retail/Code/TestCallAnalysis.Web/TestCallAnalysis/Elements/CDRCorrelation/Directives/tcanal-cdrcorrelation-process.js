﻿"use strict";

app.directive("tcanalCdrcorrelationProcess", ['UtilsService', 'VRDateTimeService',
    function (UtilsService, VRDateTimeService) {
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
            templateUrl: '/Client/Modules/TestCallAnalysis/Elements/CDRCorrelation/Directives/Templates/CDRCorrelationProcessTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;
            $scope.scopeModel = {};
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "TestCallAnalysis.BP.Arguments.CDRCorrelationAndAnalysisProcessInput,TestCallAnalysis.BP.Arguments",
                            DateTimeMargin: $scope.scopeModel.datetimeMargin,
                            TimeOutMargin: $scope.scopeModel.timeOutMargin
                        }
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);

