"use strict";

app.directive("vrAnalyticAutomatedreportSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AutomatedReportSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/AutomatedReport/Handler/Templates/AutomatedReportSettingsTemplate.html"

        };

        function AutomatedReportSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var serialNumberPartsAPI;
            var serialNumberPartsReadyDeferred = UtilsService.createPromiseDeferred();

            var context;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSerialNumberPartsReady = function (api) {
                    serialNumberPartsAPI = api;
                    serialNumberPartsReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                var serialNumberParts;
                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.data != undefined) {
                            serialNumberParts = payload.data.SerialNumberParts;
                        }
                        var promises = [];

                        var serialNumberPartsPromise = loadSerialNumberParts();
                        function loadSerialNumberParts() {
                            var serialNumberPartsLoadDeferred = UtilsService.createPromiseDeferred();
                            serialNumberPartsReadyDeferred.promise.then(function () {
                                var payload =
                                {
                                    serialNumberParts: serialNumberParts
                                };
                                VRUIUtilsService.callDirectiveLoad(serialNumberPartsAPI, payload, serialNumberPartsLoadDeferred);
                            });
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.Entities.VRAutomatedReportSettings, Vanrise.Analytic.Entities",
                        SerialNumberParts: serialNumberPartsAPI.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);