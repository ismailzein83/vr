"use strict";

app.directive("vrIntegrationFiledatasourcedefinitionDelaycheckerSizebased", ["UtilsService",
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SizeBasedCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "sizeBasedCtrl",
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

                    if (payload != undefined && payload.fileDelayChecker != undefined) {
                        $scope.scopeModel.peakDelayInterval = payload.fileDelayChecker.PeakDelayInterval;
                        $scope.scopeModel.offPeakDelayInterval = payload.fileDelayChecker.OffPeakDelayInterval;
                    }
                    else {
                        $scope.scopeModel.peakDelayInterval = "00:15:00";
                        $scope.scopeModel.offPeakDelayInterval = "00:30:00";
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Integration.MainExtensions.FileDelayChecker.SizeBasedFileDelayChecker, Vanrise.Integration.MainExtensions",
                        PeakDelayInterval: $scope.scopeModel.peakDelayInterval,
                        OffPeakDelayInterval: $scope.scopeModel.offPeakDelayInterval
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