"use strict";

app.directive("vrIntegrationFiledelaycheckerSizebased", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SizeBasedCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Integration/Directives/MainExtensions/Settings/FileDataSourceSettings/FileDelayChecker/Templates/SizeBasedFileDelayCheckerTemplate.html"
        };

        function SizeBasedCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.fileDelayCheckerSettings != undefined) {
                        $scope.scopeModel.maxPeakDelayPeriod = payload.fileDelayCheckerSettings.MaxPeakDelayPeriod;
                        $scope.scopeModel.maxOffPeakDelayPeriod = payload.fileDelayCheckerSettings.MaxOffPeakDelayPeriod;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Integration.MainExtensions.FileDelayChecker.SizeBasedFileDelayChecker, Vanrise.Integration.MainExtensions",
                        MaxPeakDelayPeriod: $scope.scopeModel.maxPeakDelayPeriod,
                        MaxOffPeakDelayPeriod: $scope.scopeModel.maxOffPeakDelayPeriod
                    };
                };

                if (ctrl.onReady != null && typeof ctrl.onReady == "function") {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }
]);