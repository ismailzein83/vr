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

            var fileNamePartsAPI;
            var fileNamePartsReadyDeferred = UtilsService.createPromiseDeferred();

            var context;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFileNamePartsReady = function (api) {
                    fileNamePartsAPI = api;
                    fileNamePartsReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                var fileNameParts;
                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.data != undefined) {
                            fileNameParts = payload.data.FileNameParts;
                        }
                        var promises = [];

                        var fileNamePartsPromise = loadFileNameParts();
                        function loadFileNameParts() {
                            var fileNamePartsLoadDeferred = UtilsService.createPromiseDeferred();
                            fileNamePartsReadyDeferred.promise.then(function () {
                                var payload =
                                {
                                    fileNameParts: fileNameParts
                                };
                                VRUIUtilsService.callDirectiveLoad(fileNamePartsAPI, payload, fileNamePartsLoadDeferred);
                            });
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.Entities.VRAutomatedReportSettings, Vanrise.Analytic.Entities",
                        FileNameParts: fileNamePartsAPI.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);