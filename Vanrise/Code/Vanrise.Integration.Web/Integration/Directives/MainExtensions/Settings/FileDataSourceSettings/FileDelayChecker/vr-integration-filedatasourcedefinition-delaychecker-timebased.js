"use strict";

app.directive("vrIntegrationFiledatasourcedefinitionDelaycheckerTimebased", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TimeBasedCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "timeBasedCtrl",
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

                    $scope.scopeModel.delayInterval = "00:15:00";

                    if (payload != undefined && payload.fileDelayChecker != undefined) {
                        $scope.scopeModel.delayInterval = payload.fileDelayChecker.DelayInterval;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Integration.MainExtensions.FileDelayChecker.TimeBasedFileDelayChecker, Vanrise.Integration.MainExtensions",
                        DelayInterval: $scope.scopeModel.delayInterval
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;
    }
]);