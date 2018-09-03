"use strict";

app.directive("vrAnalyticAutomatedreportFilegenerator", ['UtilsService', 'VRAnalytic_AutomatedReportProcessScheduledService', 'VRUIUtilsService', 'VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService', "VR_Analytic_AutomatedReportQuerySourceEnum", "VR_Analytic_QueryHandlerValidatorResultEnum",
    function (UtilsService, VRAnalytic_AutomatedReportProcessScheduledService, VRUIUtilsService, VR_Analytic_AutomatedReportQueryDefinitionSettingsAPIService, VR_Analytic_AutomatedReportQuerySourceEnum, VR_Analytic_QueryHandlerValidatorResultEnum) {
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
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/VRAutomatedReportFileGeneratorTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var fileGeneratorEntity;
            var fileNamePatternAPI;
            var fileNamePatternReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var fileGeneratorSelectorAPI;
            var fileGeneratorSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onFileNamePatternReady = function (api) {
                    fileNamePatternAPI = api;
                    fileNamePatternReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFileGeneratorSelectorReady = function (api) {
                    fileGeneratorSelectorAPI = api;
                    fileGeneratorSelectorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.getData = function () {
                    return {
                        Name: fileNamePatternAPI.getData(),
                        Settings: fileGeneratorSelectorAPI.getData()
                    };
                };

                api.load = function (payload) {                    
                    var name;
                    var settings;
                    if (payload != undefined){
                        context = payload.context;
                        fileGeneratorEntity = payload.fileGenerator;
                    }

                    if (fileGeneratorEntity != undefined) {

                        settings = fileGeneratorEntity.Settings;
                        name = fileGeneratorEntity.Name;
                    }
                    function loadFileNamePatternQuery() {
                        var fileNamePatternLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        fileNamePatternReadyPromiseDeferred.promise.then(function () {
                            var fileNamePatternPayload = {
                                fileNamePattern: name
                            };
                            VRUIUtilsService.callDirectiveLoad(fileNamePatternAPI, fileNamePatternPayload, fileNamePatternLoadPromiseDeferred);

                        });
                        return fileNamePatternLoadPromiseDeferred.promise;
                    }
                    function loadFileGeneratorSelector() {
                        var fileGeneratorSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        fileGeneratorSelectorReadyPromiseDeferred.promise.then(function () {
                            var fileGeneratorSelectorPayload = {
                                fileGenerator: settings,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(fileGeneratorSelectorAPI, fileGeneratorSelectorPayload, fileGeneratorSelectorLoadPromiseDeferred);

                        });
                        return fileGeneratorSelectorLoadPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises([loadFileNamePatternQuery(), loadFileGeneratorSelector()]);
                };

                api.reload = function (newQueries) {
                    fileGeneratorSelectorReadyPromiseDeferred.promise.then(function () {
                        if (fileGeneratorSelectorAPI.reload!=undefined && typeof(fileGeneratorSelectorAPI.reload)=='function') {
                            fileGeneratorSelectorAPI.reload(newQueries);
                        }
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }
        return directiveDefinitionObject;
    }]);
