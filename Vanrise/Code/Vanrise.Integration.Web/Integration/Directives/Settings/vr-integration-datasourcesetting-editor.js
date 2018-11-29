"use strict";

app.directive("vrIntegrationDatasourcesettingEditor", ["UtilsService", "VRValidationService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRValidationService, VRNotificationService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataSourceSettingDataCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Integration/Directives/Settings/Templates/DataSourceSettingTemplate.html"
        };

        function DataSourceSettingDataCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var fileDataSourceDirectiveAPI;
            var fileDataSourceDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onFileDataSourceSettingsDirectiveReady = function (api) {
                    fileDataSourceDirectiveAPI = api;
                    fileDataSourceDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var fileDataSourceSettings;

                    if (payload != undefined && payload.data != undefined) {
                        fileDataSourceSettings = payload.data.FileDataSourceSettings;
                    }

                    var loadFileDataSourceDirectivePromise = loadFileDataSourceDirective();
                    promises.push(loadFileDataSourceDirectivePromise);

                    function loadFileDataSourceDirective() {
                        var fileDataSourceDirectiveLoadPromise = UtilsService.createPromiseDeferred();

                        fileDataSourceDirectiveReadyDeferred.promise.then(function () {
                            var payload;
                            if (fileDataSourceSettings != undefined) {
                                payload = { fileDataSourceSettings: fileDataSourceSettings };
                            }
                            VRUIUtilsService.callDirectiveLoad(fileDataSourceDirectiveAPI, payload, fileDataSourceDirectiveLoadPromise);
                        });

                        return fileDataSourceDirectiveLoadPromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Integration.Entities.DataSourceSettingData, Vanrise.Integration.Entities",
                        FileDataSourceSettings: fileDataSourceDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null && typeof ctrl.onReady == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);