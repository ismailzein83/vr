"use strict";

app.directive("vrIntegrationFiledelaycheckerTimebased", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TimeBasedCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Integration/Directives/MainExtensions/Settings/FileDataSourceSettings/FileDelayChecker/Templates/TimeBasedFileDelayCheckerTemplate.html"
        };

        function TimeBasedCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.fileDelayCheckerSettings != undefined) {
                        $scope.scopeModel.maxDelayPeriod = payload.fileDelayCheckerSettings.MaxDelayPeriod;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Integration.MainExtensions.FileDelayChecker.TimeBasedFileDelayChecker, Vanrise.Integration.MainExtensions",
                        MaxDelayPeriod: $scope.scopeModel.maxDelayPeriod                      
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